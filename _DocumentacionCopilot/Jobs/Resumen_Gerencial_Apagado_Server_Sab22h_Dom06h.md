# Resumen Ejecutivo: Apagado del Server Productivo (Sáb 22:00 – Dom 06:00)

**Para**: Gerencia / Negocio
**De**: Equipo DataAgro
**Objetivo**: Brindar la información necesaria para decidir si se apaga el server productivo de AWS los fines de semana (sábado 22:00 a domingo 06:00) con el fin de ahorrar costos. **La decisión final es del negocio.**

> Análisis técnico detallado disponible en: `Jobs_Hangfire_ventana_Sab22h_Dom06h.md`

---

## ¿Qué implica apagar el server en ese horario?

1. **La aplicación DataAgro completa deja de estar disponible.** Nadie podrá ingresar, consultar ni cargar información (negocios, contratos, acuerdos, cupos, etc.) durante esas 8 horas.
2. **Los procesos automáticos (jobs) que corren en ese horario no se ejecutan.** La mayoría se recupera solo, actualizando el dato en la corrida siguiente. **Tres procesos, sin embargo, no se recuperan automáticamente y requerirían una acción manual del equipo técnico** para no perder información.

---

## Puntos que el negocio debe confirmar antes de decidir

- [ ] **¿Hay necesidad real de operar en DataAgro entre el sábado 22:00 y el domingo 06:00?** (comerciales, jefes, guardias, etc.)
- [ ] **¿Alguna integración externa** (Primary/MAT, SISA, FACACOP, u otras) envía o necesita recibir datos de/hacia DataAgro en ese horario?
- [ ] Si la respuesta a ambos puntos es "no", el apagado es viable con bajo riesgo operativo.

---

## Impacto en los procesos automáticos (jobs)

| Proceso | Riesgo | Impacto si se apaga el server |
|---|---|---|
| Mail de alerta "Negocios anulados y reemplazados" | 🔴 **Alto** | No se envía el aviso de control correspondiente al sábado; requiere reenvío manual. |
| Reporte "Compra Net" | 🔴 **Alto** | El reporte del sábado no se genera; se pierde ese día del histórico salvo generación manual. |
| Cumplimiento de cupos | 🟠 **Medio-alto** | El cumplimiento de cupos del sábado puede quedar sin actualizar hasta que se fuerce manualmente. |
| Finalización automática de contratos | 🟡 **Medio** | Contratos que debían cerrarse el sábado a la noche quedan "vigentes" un par de días de más (se corrige solo el lunes). |
| Anulación de acuerdos vencidos | 🟢 **Bajo-medio** | Se anulan igual, con un par de días de demora. |
| Resto de los procesos (actualización de precios, cupos, proveedores, scoring, pesificados, etc.) | 🟢 **Bajo** | Solo quedan "desactualizados" durante la ventana; se corrigen automáticamente al reactivarse. |
| Migración de contratos a Primary | ⚪ **Ninguno** | Ya está programado para no correr los fines de semana, independientemente del apagado. |

**Nota importante**: como durante el apagado nadie puede cargar información nueva en DataAgro, los datos que faltarían procesar son únicamente los que ya se cargaron el sábado antes de las 22:00. Esto permite que, para los 3 casos de riesgo alto/medio-alto, el equipo técnico pueda ejecutar manualmente esos procesos el lunes a primera hora con la fecha del sábado, sin pérdida real de información, aunque **requiere una tarea adicional del equipo técnico cada lunes** (o automatizar ese catch-up).

---

## Alternativas para reducir el riesgo

1. **Automatizar el "catch-up"**: que el sistema, al reactivarse, detecte y ejecute automáticamente los procesos que quedaron pendientes de la fecha del sábado (recomendado, requiere desarrollo).
2. **Achicar la ventana de apagado**: por ejemplo, de 23:55 a 04:55 en vez de 22:00 a 06:00, reduciendo el ahorro de costos pero minimizando el impacto en los procesos críticos.
3. **Apagar solo algunos días**, o dejar prendido el server los sábados a la noche (que es donde se concentran los procesos de mayor riesgo) y apagarlo solo de madrugada del domingo.
4. **Aceptar el impacto**: dado que es de bajo riesgo real (no hay pérdida de datos de negocio, solo demora en reportes/alertas), aprobar el apagado tal cual está planteado y que el equipo técnico gestione manualmente los 3 procesos críticos cada lunes.

---

## Recomendación del equipo DataAgro

Desde el punto de vista técnico, el apagado del server en esa ventana **es viable y de bajo riesgo operativo**, siempre que:
- se confirme que no hay operatoria de negocio ni integraciones externas activas en ese horario, y
- se acuerde un procedimiento (manual o automatizado) para los 3 procesos de riesgo alto/medio-alto.

La decisión final sobre si el ahorro de costos justifica este esquema y el procedimiento de catch-up queda a criterio del negocio.
