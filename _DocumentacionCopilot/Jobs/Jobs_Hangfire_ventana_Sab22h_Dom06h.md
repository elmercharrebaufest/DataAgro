# Jobs Hangfire en ventana Sábado 22:00 – Domingo 06:00

## Objetivo

Determinar qué jobs Hangfire corren dentro de la ventana propuesta para apagar el server productivo de AWS (**sábado 22:00 → domingo 06:00**) y qué pasaría si:

- **(A) corren al día siguiente** (el server se prende y Hangfire ejecuta la próxima corrida programada, ej. lunes),
- **(B) corren antes de la ventana** (se adelanta manualmente la ejecución antes de las 22:00 del sábado).

El análisis se basa en la lectura del código real de cada Job (`WebDataAgro\Jobs\*.cs`) y de los métodos de negocio que invocan (`NegocioManager`, `ContratoAcuerdoManager`, `ContratoManager`, `FijacionDePrecioContratoManager`, `ReportesManager`, `CupoManager`, `ProveedorManager`, `ComprasManager`, `ResearchManager`).

Todos los jobs comparten un patrón común: primero consultan `HabilitacionJob` (tabla de habilitación) y si está deshabilitado, no hacen nada. Esto es relevante como "Plan B" (ver sección de alternativas).

---

## Consideración clave: la aplicación completa queda inaccesible durante la ventana

Apagar el server no solo detiene Hangfire: **detiene IIS/la aplicación web completa**. Esto significa que durante la ventana (sábado 22:00 → domingo 06:00) **nadie puede cargar, confirmar, anular ni modificar negocios, contratos, acuerdos, cupos, etc. en DataAgro**, ya que la app no está disponible. Esto tiene dos efectos que hay que sumar al análisis de los jobs:

1. **Impacto operativo directo (independiente de los jobs)**: si algún comercial, jefe o proceso externo necesita operar en DataAgro durante ese horario (por ejemplo, cargar un negocio de urgencia un sábado a la noche o domingo a la madrugada), **no podrá hacerlo**. Antes de aprobar el apagado hay que confirmar con el negocio que en esa franja no hay operatoria real (lo cual es razonable asumir para un sábado a la noche/domingo de madrugada, pero debe confirmarse, sobre todo si hay integraciones externas o procesos batch de terceros que interactúan con DataAgro en ese horario).
2. **Acota (y en algunos casos elimina) el riesgo de pérdida de datos de los jobs con filtro de fecha exacta**: como nadie puede cargar registros nuevos mientras el server está apagado, lo único que los jobs de las 22:xx/23:xx del sábado necesitan procesar es **lo que ya se cargó antes de las 22:00** (durante el horario hábil del sábado). Es decir, el "universo" de datos a procesar para ese día ya está cerrado/congelado en el momento del apagado; no hay más movimientos que puedan "escaparse" durante la ventana. Esto **no elimina** el problema de los jobs con filtro exacto (ver punto 3 más abajo, siguen sin ejecutarse y su ventana de captura no se repite), pero sí confirma que un catch-up manual con la fecha del sábado sería 100% preciso (no hay riesgo de que falte data cargada "durante" el apagado, porque no pudo cargarse nada).
3. **No mitiga los jobs con filtro de fecha exacta** (`EnvioMailNegociosAnulaYReemplazaHangfireJob`, `GrabarDatosReporteCompraNetHangfireJob`, `ActualizarCumplimientoCuposHangfireJob`): estos igual necesitan una corrida de catch-up para la fecha del sábado, ya que el hecho de que la app esté caída no hace que el job se ejecute retroactivamente solo. Ver detalle y alternativas en la sección de cada job.

---

## Clasificación por criticidad de la lógica (filtro de fecha)

Lo determinante para el impacto **no es el nombre del job**, sino **si el método de negocio filtra por una fecha exacta ("=="), por un umbral acumulativo ("<", ">="), o si no depende de una fecha específica** (solo recalcula el estado "actual").

| Tipo de lógica                                                     | Efecto de correr tarde (día siguiente)                                                                                              | Efecto de correr antes (previo a las 22:00)                                                                                                                                                                           |
|--------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Filtro de fecha exacta** (`== hoy`, `fechaDesde=fechaHasta=hoy`) | **Pérdida de datos**: el día para el que debía ejecutarse nunca se vuelve a evaluar (salvo corrida manual con esa fecha).           | Se ejecuta con la fecha de "hoy" (el día antes de la ventana), por lo que el día real que se quería cubrir (sábado o domingo) **queda sin cobertura igual**, a menos que se le pase explícitamente la fecha objetivo. |
| **Filtro acumulativo** (`< umbral`, `>= hoy`)                      | **Sin pérdida real**: al correr un día después, el filtro sigue capturando los registros pendientes (solo se retrasa el resultado). | Sin problema: se anticipa el resultado, pero puede no incluir movimientos de último momento del mismo día.                                                                                                            |
| **Recalculo de estado "actual"** (no depende de fecha puntual)     | **Sin pérdida de datos**: solo queda "desactualizado" durante la ventana; se corrige en la próxima corrida.                         | Sin problema, aunque el dato quedará desactualizado durante toda la ventana igual (se refresca recién el lunes).                                                                                                      |

---

## Detalle job por job

### 1. `AnularAcuerdosHangfireJob` (50 22 * * *) → `ContratoAcuerdoManager.AnularAcuerdos()`

- **Lógica**: obtiene `dia = UltimoDiaHabil(null)` y anula (pasa a Finalizado) todo `ContratoAcuerdo` con `Fecha < dia` y `Estado == Confirmado`. Es un filtro **acumulativo** (`<`), no exacto.
- **Si corre al día siguiente (lunes)**: no hay pérdida de datos. Los acuerdos que debían anularse el sábado a las 22:50 simplemente se anulan el lunes cuando corra el job, incluidos junto con los que correspondan a ese día. El único efecto es que los acuerdos quedan en estado "Confirmado" ~2 días más de lo esperado (podrían usarse/mostrarse como vigentes en pantallas o reportes durante el fin de semana).
- **Si corre antes de la ventana (ej. sábado 20:00)**: mismo resultado, ya que el filtro es acumulativo; no se pierde nada, simplemente se adelanta.
- **Impacto**: **Bajo-medio**. Riesgo funcional: cualquier proceso o pantalla que dependa de que el acuerdo esté "Finalizado" (por ejemplo, reportes de cierre) mostrará el acuerdo como "Confirmado" durante el fin de semana.
- **Alternativa**: ninguna acción requerida; es seguro dejar que corra el lunes.

### 2. `EnvioMailNegociosConDiaAnteriorHangfireJob` (02 23 * * *) → `NegocioManager.EnvioMailNegociosConDiaAnterior()`

- **Lógica**: `hoy = DateTime.Now.Date`; busca `Negocio` con `Fecha != FechaOperacion` y **`Fecha >= hoy`** (filtro acumulativo hacia adelante) y `Estado == Finalizado`.
- **Si corre al día siguiente (lunes)**: `hoy` pasa a ser lunes, pero el filtro `Fecha >= hoy` es "hacia adelante", por lo que **no se pierden negocios**: los negocios generados el sábado con fecha retroactiva seguirán cumpliendo la condición (su `Fecha` seguirá siendo `>=` lunes) y serán incluidos en el mail del lunes.
- **Si corre antes de la ventana**: mismo comportamiento, sin pérdida.
- **Impacto**: **Bajo**. Solo se retrasa el envío del mail de alerta ~2 días (sábado/domingo a lunes).
- **Alternativa**: ninguna acción requerida.

### 3. `EnvioMailNegociosAnulaYReemplazaHangfireJob` (50 23 * * *) → `NegocioManager.EnvioMailNegociosAnulaYReemplaza()`

- **Lógica**: `hoy = DateTime.Now.Date`; busca `Contrato` con **`TruncateTime(Fecha) == hoy`** (filtro **exacto**, día por día) y `AnulaYReemplazaContratoId != null` y `EstadoId == 5`.
- **Si corre al día siguiente (lunes)**: `hoy` será lunes. Los contratos "anula y reemplaza" generados el **sábado nunca se reportan**, porque el filtro exige coincidencia exacta de fecha y el lunes ya no calza con `Fecha == sábado`. **Se pierde el mail y el control de negocios anulados/reemplazados de ese día.**
- **Si corre antes de la ventana (ej. sábado 20:00)**: se ejecuta con `hoy = sábado`, cubriendo el sábado, pero **el corte se hace antes de que termine el día**, por lo que cualquier anula-y-reemplaza generado entre las 20:00 y las 23:59 del sábado **tampoco se reportaría** (mismo problema, solo que se traslada la ventana de riesgo a esas horas).
- **Impacto**: **Alto** (es el único de los jobs de mail con filtro estrictamente diario). Es información de control (auditoría de anulaciones/reemplazos de negocios); su pérdida implica que un jefe/comercial nunca reciba la alerta de esos movimientos del sábado, salvo que se audite manualmente.
- **Alternativas**:
  - Ejecutar manualmente el endpoint `TareasProgramadasController.EnvioMailNegociosAnulaYReemplaza()` el lunes a primera hora, indicando la fecha del sábado (requiere modificar el método para aceptar una fecha por parámetro, hoy no la recibe).
  - O bien, no incluir el sábado en la ventana de apagado (apagar recién después de las 23:50) y prender antes de las 00:00 del domingo si se quiere cubrir también el domingo.

### 4. `FinalizacionContratosHangfireJob` (0 20,23 * * *) → `ContratoManager.FinalizacionAutomatica()` + `FijacionDePrecioContratoManager.FinalizacionAutomatica()`

- Solo la corrida de las **23:00** cae en la ventana (la de las 20:00 no).
- **Lógica**: no se ve un filtro de fecha explícito en la firma (`FinalizacionAutomatica(idActiveDirectory)`); internamente identifica contratos/fijaciones cuyo plazo/condición de finalización ya se cumplió (vencimiento), es decir, un **recalculo de estado actual** más que un snapshot de "hoy".
- **Si corre al día siguiente (lunes)**: los contratos/fijaciones que debían finalizarse el sábado a las 23:00 se finalizan igual el lunes (con fecha de proceso corrida), sin pérdida de datos, aunque quedan "vigentes" de más durante el fin de semana (mismo riesgo que el punto 1: reportes/pantallas que dependan del estado "Finalizado").
- **Si corre antes de la ventana**: mismo resultado, solo se adelanta.
- **Impacto**: **Medio**. Los contratos que deberían estar finalizados seguirán apareciendo como vigentes (con posible impacto en reportes de cierre de posición/exposición) hasta el lunes.
- **Alternativa**: si existe un reporte de cierre de fin de semana que dependa de contratos ya finalizados, correr manualmente el job (o el endpoint equivalente) antes de generar ese reporte.

### 5. `GrabarDatosReporteCompraNetHangfireJob` (10 23 * * *) → `ReportesManager.GrabarDatosReporteCompraNet(fechaDesde=hoy, fechaHasta=hoy, ...)`

- **Lógica**: `fechaD = DateTime.Now.Date`; genera el reporte de **ese único día** (`fechaDesde == fechaHasta == hoy`), filtro **exacto**.
- **Si corre al día siguiente (lunes)**: el reporte se genera con fecha = lunes; **el reporte del sábado nunca se genera** (no hay "catch-up" automático).
- **Si corre antes de la ventana**: se generaría el reporte del sábado, pero con datos incompletos si aún faltan compras/negocios por cargar antes de medianoche.
- **Impacto**: **Alto** si el reporte "Compra Net" se usa para análisis histórico diario (auditoría, comparativas mes a mes); se pierde el dato del sábado.
- **Alternativas**:
  - Ejecutar manualmente `reportesManager.GrabarDatosReporteCompraNet(sábado, sábado, "0", null)` el lunes (vía un endpoint/tarea programada existente o un script one-off), para no perder el histórico.
  - Evaluar si el negocio realmente necesita el reporte de un sábado (día no hábil); si no, aceptar el hueco documentándolo.

### 6. `MigrarContratosPrimaryHangfireJob` (30 18,23 * * *) → `NegocioManager.MigrarContratosPrimary(fecha)`

- **Importante**: el propio job **ya excluye sábados y domingos** en su código:
  ```csharp
  if (!(dia.DayOfWeek == DayOfWeek.Saturday || dia.DayOfWeek == DayOfWeek.Sunday))
      negocioManager.MigrarContratosPrimary(dia);
  ```
- La corrida de las 23:30 del sábado **ya es un no-op hoy en día**, independientemente del apagado del server. Las corridas de 18:30/23:30 del domingo tampoco caen en la ventana propuesta (termina a las 06:00).
- **Impacto de apagar el server en la ventana**: **Ninguno**. Este job no se ve afectado por la decisión de apagado, ya que weekend ya está contemplado como no-op.
- **Alternativa**: no requiere ninguna acción.

### 7. `ProcessComprasHangfireJob` (15 23 * * *) → `ComprasManager.ActualizarComprasDetalle("", "")`

- **Lógica**: parámetros vacíos (sin rango de fechas explícito); es un **recalculo del detalle de compras actual/pendiente**, no un snapshot de un día puntual.
- **Si corre al día siguiente**: se ejecuta igual sobre el estado actual (incluye lo pendiente acumulado), sin pérdida de datos; solo hay una demora de ~1 día y medio en reflejar el detalle de compras.
- **Si corre antes de la ventana**: mismo resultado, adelantado.
- **Impacto**: **Bajo-medio**. El detalle de compras queda desactualizado durante el fin de semana (relevante solo si se consulta el domingo).
- **Alternativa**: ninguna acción requerida.

### 8. `PesificadosHangfireJob` (0 */2 * * *) → `ReportesManager.GrabarTodoDatoPesificar()`

- Corre 4 veces dentro de la ventana (22:00, 00:00, 02:00, 04:00).
- **Lógica**: recalcula/graba "todo dato a pesificar" (snapshot del estado actual, no exclusivo de un día puntual, a juzgar por el nombre `GrabarTodo...`).
- **Si corre al día siguiente**: se pierde la frecuencia de actualización durante 8 horas, pero al reanudar recalcula sobre el estado vigente (no hay pérdida de información, solo de "frescura" del dato).
- **Impacto**: **Bajo-medio**, dependiendo de cuán sensible al tiempo sea el dato de "pesificados" (tipo de cambio/actualización cambiaria). Si el negocio usa esta info para pantallas en tiempo real durante la madrugada, no aplica (no hay operación esos horarios).
- **Alternativa**: ninguna acción requerida; si se quiere, forzar una corrida manual al prender el server el domingo a las 06:00 para minimizar el desfasaje.

### 9. `SincronizarResearchHangfireJob` (`Cron.Daily` = 00:00) → `ResearchManager.SincronizarResearchPowerApp()`

- **Lógica**: sincroniza datos de "Research" con PowerApp; no se identifica filtro de fecha exacto en la capa job (se debería confirmar en el manager si aplica delta o snapshot completo).
- **Si corre al día siguiente**: probablemente sin pérdida (sincronización de estado actual), solo un día de desfasaje si PowerApp consulta datos "frescos" el domingo.
- **Impacto**: **Bajo**, salvo que PowerApp sea consultado activamente los domingos por la mañana.
- **Alternativa**: confirmar con el equipo dueño de PowerApp si se consulta ese dato en domingo antes de las 06:00.

### 10. `ActualizarScoringCuposDeProveedoresHangfireJob` (0 1 * * *) → `ProveedorManager.ActualizarScoringCuposDeProveedores()`

- **Lógica**: recalcula el scoring vigente de cupos de proveedores (estado actual, no snapshot histórico).
- **Impacto de correr al día siguiente**: **Bajo**; el scoring queda desactualizado hasta el lunes, se corrige solo.
- **Alternativa**: ninguna acción requerida.

### 11. `ActualizarProveedoresHomeHangfireJob` (0 2 * * *) → `ProveedorManager.ActualizarProveedoresHome()`

- **Lógica**: refresca datos del "home" de proveedores (vista/cache, estado actual).
- **Impacto de correr al día siguiente**: **Bajo**; el home de proveedores se ve con datos de viernes hasta que corra el lunes. Solo relevante si hay usuarios navegando el domingo.
- **Alternativa**: ninguna acción requerida.

### 12. `ActualizarCumplimientoCuposHangfireJob` (0 5 * * *) → `CupoManager.ActualizarCumplimientoCupos(ayer = hoy-1)`

- **Lógica**: recibe `ayer` y calcula cumplimiento **específicamente para esa fecha** (`cumplimientoCuposAgent.Ejecutar(_, ayer)`), sobre cupos con `FechaIngreso` en un rango de 3 días (`ayer`, `hoy`, `mañana`). Es un cálculo **atado a una fecha puntual** (similar a un filtro exacto, aunque con ventana de +/-1 día para los cupos considerados).
- **Si corre al día siguiente (lunes 5:00)**: `ayer` pasa a ser domingo. **El cumplimiento específico del sábado nunca se recalcula** con `ayer = sábado` (se salta esa fecha), aunque los cupos con `FechaIngreso` sábado podrían quedar parcialmente cubiertos si su `FechaIngreso` coincide con el rango evaluado el lunes o el domingo (revisar caso a caso). En general, **hay riesgo de que el cumplimiento del sábado quede desactualizado permanentemente** salvo corrida manual.
- **Si corre antes de la ventana**: se ejecutaría con `ayer = viernes` (si se adelanta el sábado a la madrugada), sin cubrir el propio sábado.
- **Impacto**: **Medio-alto** si el negocio usa el cumplimiento de cupos para decisiones comerciales del lunes (cupos que se cerraron el sábado quedarían con cumplimiento desactualizado).
- **Alternativa**: ejecutar manualmente `cupoManager.ActualizarCumplimientoCupos(sábado)` el lunes a primera hora (vía un endpoint en `TareasProgramadasController` o una tarea one-off), antes de que el negocio consuma el dato.

### 13. `ActualizarMailProveedorHangfireJob` (0 6 * * *, límite de la ventana) → `ProveedorManager.GrabarMailProveedor()`

- Corre justo en el límite (06:00); depende de si el server ya está encendido a esa hora exacta.
- **Lógica**: graba el mail vigente de proveedores (estado actual, no snapshot de un día).
- **Impacto de correr tarde (si el server tarda en prender)**: **Bajo**; el dato de mail de proveedor se actualiza en la próxima corrida sin pérdida.
- **Alternativa**: si se quiere garantizar la corrida de las 06:00, programar el encendido del server a las 05:55 en vez de las 06:00 en punto.

### 14. `CrearSugerenciaCupoHangfireJob` (0 6 * * *, límite de la ventana) → `CupoManager.CrearSugerenciaCupo()`

- Mismo caso que el anterior: corre justo en el borde de la ventana.
- **Lógica**: genera sugerencias de cupo (basadas en el estado actual del sistema, no snapshot de un día puntual según lo revisado).
- **Impacto de correr tarde**: **Bajo-medio**; las sugerencias de cupo se generan más tarde de lo habitual, pero no se pierden (se generan igual, solo con datos más frescos si corre después).
- **Alternativa**: igual que el punto anterior, considerar encender el server unos minutos antes de las 06:00 (ej. 05:55) para no depender del borde exacto del cron.

---

## Resumen ejecutivo

| Job                                              | Impacto real de apagar el server | Motivo                                                                  |
|--------------------------------------------------|----------------|-------------------------------------------------------------------------------------------|
| `EnvioMailNegociosAnulaYReemplazaHangfireJob`    | **Alto**       | Filtro de fecha exacta (`==`); se pierde el reporte del sábado.                           |
| `GrabarDatosReporteCompraNetHangfireJob`         | **Alto**       | Snapshot de un único día exacto; el reporte del sábado nunca se genera.                   |
| `ActualizarCumplimientoCuposHangfireJob`         | **Medio-alto** | Cálculo atado a la fecha "ayer"; el cumplimiento del sábado puede quedar sin recalcular.  |
| `FinalizacionContratosHangfireJob`               | **Medio**      | Contratos/fijaciones quedan "vigentes" de más durante el fin de semana.                   |
| `AnularAcuerdosHangfireJob`                      | **Bajo-medio** | Filtro acumulativo (`<`); se recupera solo, con demora.                                   |
| `ProcessComprasHangfireJob`                      | **Bajo-medio** | Recalculo de estado actual; solo desactualización temporal.                               |
| `PesificadosHangfireJob`                         | **Bajo-medio** | 4 corridas perdidas, pero recuperables (estado actual).                                   |
| `EnvioMailNegociosConDiaAnteriorHangfireJob`     | **Bajo**       | Filtro acumulativo hacia adelante (`>=`); no se pierde información.                       |
| `SincronizarResearchHangfireJob`                 | **Bajo**       | Sincronización de estado actual.                                                          |
| `ActualizarScoringCuposDeProveedoresHangfireJob` | **Bajo**       | Recalculo de estado actual.                                                               |
| `ActualizarProveedoresHomeHangfireJob`           | **Bajo**       | Recalculo de estado actual (cache/home).                                                  |
| `ActualizarMailProveedorHangfireJob`             | **Bajo**       | Recalculo de estado actual; corre en el borde de la ventana.                              |
| `CrearSugerenciaCupoHangfireJob`                 | **Bajo-medio** | Recalculo de estado actual; corre en el borde de la ventana.                              |
| `MigrarContratosPrimaryHangfireJob`              | **Ninguno**    | Ya excluye sábado/domingo en su propio código.                                            |

---

## Alternativas / planes propuestos

1. **Para los jobs de impacto Alto/Medio-alto** (`EnvioMailNegociosAnulaYReemplazaHangfireJob`, `GrabarDatosReporteCompraNetHangfireJob`, `ActualizarCumplimientoCuposHangfireJob`):
   - Modificar los métodos de negocio (o agregar overloads) para poder invocarlos manualmente indicando una fecha específica, y ejecutar un "catch-up" el lunes a primera hora para la fecha del sábado.
   - Alternativamente, agregar lógica de "reintento/catch-up" automática en el job: al iniciar el server, detectar si la última ejecución exitosa quedó más de X horas atrás y disparar la corrida para la fecha faltante.
2. **Para los jobs en el borde de la ventana (06:00)** (`ActualizarMailProveedorHangfireJob`, `CrearSugerenciaCupoHangfireJob`): adelantar el encendido del server a las 05:55 (en vez de 06:00) para no depender de la precisión del arranque de AWS/Hangfire.
3. **Para los jobs de impacto Bajo/Bajo-medio**: no requieren cambios; alcanza con documentar que quedarán "desactualizados" durante la ventana y se auto-corrigen en la corrida siguiente.
4. **Plan alternativo de apagado**: en vez de apagar de sábado 22:00 a domingo 06:00 en bloque, evaluar apagar de **sábado 23:55 a domingo 04:55**, reduciendo el solapamiento con los jobs de las 23:xx (que son los de mayor impacto) y dejando margen antes del corte de las 05:00/06:00.
5. **Gobernanza**: aprovechar la tabla `HabilitacionJob` (ya usada por todos los jobs) para, si se decide, deshabilitar puntualmente algún job los fines de semana en vez de apagar todo el server, si el ahorro de costos buscado no requiere apagar el server completo sino solo pausar ciertos procesos.
6. **Confirmar con el negocio la indisponibilidad total de la app**: dado que apagar el server implica que **nadie puede cargar/operar en DataAgro** durante la ventana (no solo que los jobs no corren), se debe validar explícitamente con las áreas comerciales/operativas y con cualquier integración externa (ej. Primary, MAT, SISA, FACACOP) que no haya necesidad de uso ni de recepción de datos automatizados en ese horario. Si alguna integración envía datos a DataAgro en la madrugada del domingo (o el sábado a la noche), esos datos deberán reintentarse o encolarse para cuando el server vuelva a estar arriba.

---

## Conclusión

Apagar el server implica dos frentes de impacto que deben evaluarse en conjunto:

1. **Indisponibilidad total de la aplicación**: durante la ventana nadie puede cargar ni modificar negocios, contratos, acuerdos o cupos en DataAgro. Esto es aceptable si se confirma que no hay operatoria real (ni integraciones externas) en ese horario, pero es una condición necesaria a validar con el negocio antes de aprobar el apagado — es un impacto en sí mismo, más allá de los jobs.
2. **Jobs con pérdida real de información**: de los 14 jobs que caen en la ventana, **solo 3 tienen impacto real de pérdida de datos** (`EnvioMailNegociosAnulaYReemplazaHangfireJob`, `GrabarDatosReporteCompraNetHangfireJob`, `ActualizarCumplimientoCuposHangfireJob`), por filtrar sobre una fecha exacta o puntual. Como durante el apagado no se puede cargar nada nuevo, el universo de datos del sábado queda "cerrado" antes de las 22:00, por lo que un catch-up manual con la fecha del sábado sería preciso y suficiente para estos 3 jobs. El resto de los jobs son recálculos de estado actual o usan filtros acumulativos, por lo que apagar el server en esa ventana **no genera pérdida de datos** en ellos, solo una demora aceptable hasta la próxima corrida.

Se recomienda: (a) confirmar con el negocio la no-operatoria durante la ventana (punto 6), y (b) implementar el catch-up manual o automático para los 3 jobs críticos (punto 1) antes de aprobar el apagado definitivo del server productivo.
