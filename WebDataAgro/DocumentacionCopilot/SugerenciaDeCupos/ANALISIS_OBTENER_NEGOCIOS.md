# Análisis del Método `ObtenerNegocios`

## Propósito General
El método `ObtenerNegocios` es parte del sistema de sugerencia automática de cupos. Su función principal es **recopilar, validar y procesar datos de contratos** para generar sugerencias de cupos que se distribuirán a camiones. Es un algoritmo que filtra, calcula y limita la cantidad de cupos disponibles basándose en múltiples criterios de negocio.

## Flujo de Ejecución

### 1. **Inicialización y Filtrado Inicial de Contratos** (Líneas 1987-2045)
**Objetivo**: Obtener todos los contratos candidatos que cumplan con criterios básicos.

**Operaciones**:
- Obtiene tipos de negocio excluidos para la fórmula actual
- Obtiene contratos que tienen "Anula y Reemplaza"
- Consulta BD para traer contratos finalizados que:
  - No sean EPA, EUDR ni Fason
  - No estén excluidos por tipo de negocio
  - No tengan contrato sustituto (Anula y Reemplaza)
  - Cumplan rango de fechas de negocio (NegociosDesde - NegociosHasta)
  - Pertenezcan al centro y material especificados
  - Tengan estado "Finalizado"

**Mapeo**: Los datos se transforman a `SugerenciaCupoDto` con información comercial completa.

---

### 2. **Cálculo de Kilos Pendientes por Contrato** (Líneas 2049-2145)
**Objetivo**: Determinar la cantidad de kilogramos que cada contrato aún puede ocupar en cupos.

**Operaciones**:
- Obtiene el mínimo de kilos requeridos para sugerir un cupo
- Consulta el servicio SAP (`ContratoKgPendienteAgent.Consultar`) para obtener kilos pendientes reales
- Para cada contrato:
  - Asigna la zona según la descripción
  - Calcula cupos basado en kilos pendientes (cada cupo = 30,000 kg)
  - Maneja excedentes (si quedan >= mínimo, suma 1 cupo extra)
  - **Descuenta cupos** si hay cupos pendientes de otros ciclos
  - **Descuenta cupos** si hay solicitudes de administración pendientes

**Resultado**: Lista de contratos con `CantidadDeCupos` ajustada (puede quedar en 0)

---

### 3. **Filtrado de Contratos Warranty y CCPP Pendientes** (Líneas 2145-2247)
**Objetivo**: Identificar contratos con características especiales y descontar cupos por Cartas de Porte pendientes de aplicar.

**Operaciones**:
- Consulta los contratos que tienen garantía/CD (Warranty)
- Agrupa contratos por Proveedor y Corredor
- Para cada grupo, consulta el servicio SAP (`CartasDePortePendienteAplicarAgent`) para obtener CCPP pendientes
- Si CCPP pendientes >= mínimo de kilos:
  - Calcula cupos a descontar
  - Los descuenta proporcionalmente a cada contrato del grupo (priorizando por Canje, CDWarrant, MercsDeposito)

---

### 4. **Gestión de Stock Sustentable** (Líneas 2250-2307)
**Objetivo**: Limitar sugerencias de cupos sustentables al stock disponible en establecimientos.

**Operaciones**:
- Filtra contratos sustentables
- Obtiene stock disponible en establecimientos para cada proveedor
- Descuenta cupos sustentables ya generados
- Limita cantidad de cupos sugeridos al stock disponible
- Si no hay suficiente, ajusta la cantidad y registra en "Inhabilitado"

---

### 5. **Gestión de Stock EPA/EUDR** (Líneas 2309-2366)
**Objetivo**: Limitar sugerencias de cupos EPA/EUDR al stock disponible en establecimientos.

**Operaciones**: 
- Idéntico flujo al paso 4, pero aplicado a cupos EPA/EUDR
- Utiliza flag de EPA/EUDR en lugar de Sustentable
- Valida stock disponible con parámetro `esEPAoEUDR = true`

---

### 6. **Procesamiento de Espacio Dinámico** (Líneas 2368-2438)
**Objetivo**: Incluir configuraciones de espacio dinámico como opciones de cupo.

**Operaciones**:
- Obtiene configuraciones de espacio dinámico para fecha/centro/material
- Aplica mismo análisis de cupos pendientes
- Descuenta cupos ya asignados
- Descuenta solicitaciones pendientes

---

## Campos Importantes en SugerenciaCupoDto

| Campo                        | Origen                          | Propósito                              |
|------------------------------|---------------------------------|----------------------------------------|
| `NegocioId`                  | Contrato.Id                     | Identificador único del negocio        |
| `ContratoSAP`                | Contrato.ContratoSAP            | Código del contrato en SAP             |
| `CantidadDeCupos`            | Calculado                       | Cupos finales disponibles para sugerir |
| `KgPendienteAplicar`         | ContratoKgPendienteAgent        | Kilos que aún no se han utilizado      |
| `Inhabilitado`               | Construido                      | Razones por las que se limitó o anuló  |
| `ProveedorCUIT`              | Contrato.Proveedor/Corredor     | CUIT del proveedor o corredor          |
| `Sustentable`, `EPA`, `EUDR` | Contrato                        | Clasificación de producto              |
| `ZonaCupoId`                 | Zona encontrada por descripción | Zona de descarga                       |

---

## Validaciones Claves

1. **Validación de Kilos Mínimos**: Solo se sugieren cupos si los kilos pendientes >= mínimo configurado
2. **Descuentos Sucesivos**: Se aplican tres niveles de descuento:
   - Cupos pendientes de ciclos anteriores
   - Solicitudes de administración pendientes  
   - CCPP pendientes de aplicar
3. **Stock Disponible**: Contratos sustentables, EPA y EUDR se limitan al stock físico
4. **Exclusiones**: EPA, EUDR y Fason se excluyen del procesamiento automático

---

## Para Crear un Método Similar (Desde Excel)

### Cambios Necesarios:

1. **Entrada**: En lugar de consultar BD, recibirás una lista de `SugerenciaCupoDto` del Excel con:
   - `FechaSugerida`
   - `CantidadDeCupos`
   - `ContratoSAP`

2. **Lookup de Contrato**: Buscar el `Contrato` por `ContratoSAP` para obtener:
   - ProveedorId
   - MaterialId
   - CentroId
   - Todos los campos que no están en Excel

3. **Validaciones que SÍ necesitas**:
   - ✅ Calcular `KgPendienteAplicar` (ContratoKgPendienteAgent.Consultar)
   - ✅ Validar CCPP Pendientes (CartasDePortePendienteAplicarAgent)
   - ✅ Validar Stock Sustentable/EPA/EUDR (TraerCuposDisponiblesEnEstablecimientos)
   - ✅ Validar contra ConfiguracionCupo (LimiteCupo, LimiteAlgoritmo)
   - ✅ Validar cupos pendientes del mismo negocio
   - ✅ Validar solicitudes pendientes

4. **Validaciones que NO necesitas**:
   - ❌ Filtrado por tipo de negocio excluido (el usuario ya selecciona)
   - ❌ Filtrado de Warrant (opcional, depende de negocio)
   - ❌ Filtrado de Fason, EPA, EUDR al inicio (la entrada ya es validada)
   - ❌ Cálculo automático de cantidad (viene en Excel)
   - ❌ Ordenamiento y priorización (manual del usuario)
   - ❌ Espacio Dinámico (no aplica a entrada manual)

---

## Recomendaciones para el Método Manual

```csharp
public Resultado ValidarYProcesarSugerenciasDesdeExcel(
    List<SugerenciaCupoDto> sugerenciasExcel, 
    int centroId, 
    int materialId, 
    DateTime fechaSugerida)
{
    var resultado = new Resultado();

    // 1. Validar contra ConfiguracionCupo
    var config = ObtenerConfiguracionCupo(centroId, materialId);
    var totalSugerencias = sugerenciasExcel.Sum(x => x.CantidadDeCupos);
    if (totalSugerencias > config.LimiteAlgoritmo)
        return resultado.Error("Límite", "Supera límite de cupos");

    // 2. Para cada sugerencia:
    foreach (var sugerencia in sugerenciasExcel)
    {
        // 2a. Buscar contrato
        var contrato = ObtenerContratoSAP(sugerencia.ContratoSAP);

        // 2b. Calcular KgPendienteAplicar
        var kgPendientes = contratoKgPendienteAgent.Consultar(...);

        // 2c. Validar CCPP pendientes
        var ccppPendientes = cartasDePorte.ListarCartasDePortePendienteAplicar(...);

        // 2d. Validar stock si es Sustentable/EPA/EUDR
        if (contrato.Sustentable)
            ValidarStockSustentable(sugerencia);

        // etc.
    }

    return resultado;
}
```
