**Versión:** 1.0
**Última actualización:** 29/07/2026
**Autor:** Equipo DataAgro

---

# Integración con MATBA/ROFEX (Primary API)

Este documento describe el proceso de comunicación con el sistema externo **Primary API** (MATBA ROFEX), el job que dispara la sincronización, los campos que provee la API, cuáles de ellos utiliza Data Agro, y qué campos se comparan para determinar si existen diferencias entre Data Agro y la posición del MAT.

## 1. Disparador del proceso

**Archivo:** `WebDataAgro\Jobs\MigrarContratosPrimaryHangfireJob.cs`

- Es un Hangfire Job (`MigrarContratosPrimaryHangfireJob`) que se ejecuta de forma programada.
- Antes de ejecutar, valida en la tabla `HabilitacionJob` si el job `"MigrarContratosPrimaryHangfireJob"` está habilitado. Si no lo está, no hace nada.
- Si el día actual (`DateTime.Today`) **no** es sábado ni domingo, invoca `negocioManager.MigrarContratosPrimary(DateTime.Today)`.
- Este método es el que realiza toda la comunicación con la API externa y la comparación de negocios.

## 2. Comunicación con la API externa (Primary/MATBA ROFEX)

**Archivo:** `Molinos.DataAgro.Agent\Helpers\ClientePrimaryAPIAgent.cs` (implementa `IClientePrimaryAPIAgent`)

Flujo de comunicación:

1. **Autenticación** (`ObtenerToken` / `ReuseToken`): se llama al endpoint `AuthToken/AuthToken` con usuario/clave configurados (`UserPrimary`, `PassPrimary`, `UrlBasePrimary` en `web.config`/`app.config`) y se obtiene un `TokenPrimary`.
2. **Consulta de operaciones del día** (`GetTradeCaptureReport`): trae el `TradeCaptureReportResult`, con la lista de operaciones (`TradeCaptureReportValue`) realizadas en la fecha solicitada.
3. **Consulta del listado de instrumentos** (`SecurityList`): trae los instrumentos (`Instrument`) usados para resolver Material, Posición, Campaña y si es "Dólar Exportador".
4. Con esa información arma una lista de `AgenteCompra` (entidad de negocio interna) para ser comparada/persistida en Data Agro.

### Filtro aplicado sobre las operaciones traídas por la API

Solo se toman en cuenta las operaciones (`TradeCaptureReportValue`) que cumplen:

- `TrdCapRptSideGrp` contiene una cuenta (`Account`) igual a `"97500"` o `"281647"` (cuentas propias/Agente de Compras de Molinos Agro).
- `TrdType == 61` (tipo de operación correspondiente a Agente de Compras).
- `TrdRptStatus == "0"` (operación en estado Definitiva).
- Se descartan además las que no tengan `Posicion` resuelta (`!string.IsNullOrEmpty(a.Posicion)`).

## 3. Campos que trae la API Primary y uso en Data Agro

### 3.1 `TradeCaptureReportValue` (detalle de cada operación/trade)

| Campo API          | Descripción (según Primary)                                     | ¿Se usa en Data Agro? | Uso |
|--------------------|-----------------------------------------------------------------|----|---|
| `Currency`         | Moneda de negociación de la security                            | Sí | Determina `MonedaId` (`"USD"` → `USDM`, resto → `ARP`) |
| `ExecID`           | Identificación de la Operación de Mercado                       | No | No utilizado |
| `LastPx`           | Precio de la Operación                                          | Sí | Mapeado a `Precio` |
| `LastQty`          | Cantidad de la Operación                                        | Sí | Usado (junto a `Side`) para calcular `Cantidad` (método `ObtenerCantidad`) |
| `MarketID`         | Identificación del mercado (ROFX, XMEV, XMTB, etc.)             | No | No utilizado directamente en el mapeo |
| `MarketSegmentID`  | Segmento del mercado (Rueda, Fuera de rueda, etc.)              | No | No utilizado directamente en el mapeo |
| `OrderType`        | Tipo de orden (LIMIT, MARKET, etc.)                             | No | No utilizado |
| `SettlCurrency`    | Moneda del precio de ajuste                                     | No | No utilizado directamente |
| `SettlDate`        | Fecha de liquidación                                            | No | No utilizado directamente |
| `SettlType`        | Plazo de liquidación                                            | No | No utilizado |
| `TradeDate`        | Fecha en que se realizó la operación                            | No | No utilizado directamente (se usa `TransactTime`) |
| `TradeID`          | Identificación de boleta de la operación                        | Sí | Se guarda en `Observacion` ("Código de contrato MAT: ...") |
| `TradeNumber`      | Identificación de boleta                                        | No | No utilizado |
| `TransactTime`     | Fecha/hora de la transacción                                    | Sí | Mapeado a `Fecha` y `FechaOperacion` (fecha truncada) |
| `TrdRptStatus`     | Estado de la operación (0=Definitiva, 3=Anulada, 4=Transitoria) | Sí | Filtro de selección (solo `"0"`) y define `EstadoId` (Rechazado si `"3"`, si no Confirmado) |
| `TrdType`          | Tipo de operación                                               | Sí | Filtro de selección (solo `61`, Agente de Compras) |
| `VenueType`        | Tipo de mercado (Clearinghouse/Off-market/Registered)           | No | No utilizado |
| `Instrument`       | Detalle del instrumento negociado (lista)                       | Sí | Se usa `Instrument[0]` para resolver `MaterialId`, `Posicion`, `DolarExportador`, `CampanaId` |
| `RootParties`      | Partes/operador de la operación                                 | Sí | Se usa para resolver `Operador` (`ObtenerOperador`) |
| `TrdCapRptSideGrp` | Cuenta y lado (compra/venta) de la operación                    | Sí | Filtro por `Account` (97500/281647) y usado en `ObtenerCantidad` (signo según `Side`) |

### 3.2 `TradeCaptureReportInstrument` (dentro de `Instrument`)

| Campo API          | ¿Se usa?       | Uso                                                                                           |
|--------------------|----------------|-----------------------------------------------------------------------------------------------|
| `CFICode`          | Sí (indirecto) | Usado para resolver el instrumento contra el listado de `SecurityList`                        |
| `SecurityID`       | Sí             | Identifica el instrumento; se cruza con `SecurityList` para obtener Material/Posición/Campaña |
| `SecurityIDSource` | No             | No utilizado directamente                                                                     |

### 3.3 `TradeCaptureReportRootParties`

| Campo API           | ¿Se usa? | Uso                                                                             |
|---------------------|----------|---------------------------------------------------------------------------------|
| `RootPartyID`       | Sí       | Identifica al operador; se cruza contra la tabla `Operador` (`ObtenerOperador`) |
| `RootPartyIDSource` | No       | No utilizado                                                                    |
| `RootPartyRole`     | No       | No utilizado                                                                    |

### 3.4 `TradeCaptureReportTrdCapRptSideGrp`

| Campo API | ¿Se usa? | Uso                                                                     |
|-----------|----------|-------------------------------------------------------------------------|
| `Account` | Sí       | Filtro de selección (`97500` / `281647`)                                |
| `Side`    | Sí       | Determina el signo de la `Cantidad` (compra/venta) en `ObtenerCantidad` |

### 3.5 Campos resultantes en la entidad `AgenteCompra` (mapeo final que se persiste/compara)

A partir de los campos anteriores, `ObtenerNegocios` arma cada `AgenteCompra` con:

- `TipoNegocioId` = Agente de Compras (fijo)
- `Cantidad` (desde `LastQty` + `Side`)
- `Precio` (desde `LastPx`)
- `Fecha` / `FechaOperacion` (desde `TransactTime`)
- `MonedaId` (desde `Currency`)
- `EstadoId` (desde `TrdRptStatus`)
- `DestinoId` = 1 (fijo)
- `Observacion` (código de contrato MAT, desde `TradeID`)
- `MaterialId`, `Posicion`, `DolarExportador`, `CampanaId` (resueltos desde `Instrument` + `SecurityList`)
- `Operador` / `OperadorId` (resueltos desde `RootParties` + tabla `Operador`)
- `TipoAgenteCompraId` = 1 (MAT, fijo)
- `ComercialId` / `ComercialCreadorId` = 44 (usuario "Data Agro", fijo)
- `FechaDesde` / `FechaHasta` (calculados a partir de `Posicion`, ej. "ABR2024" → primer y último día del mes)

## 4. Comparación entre Data Agro y MATBA (`NegocioManager.MigrarContratosPrimary`)

**Archivo:** `Molinos.DataAgro.Business\Managers\NegocioManager.cs`

### 4.1 Origen de los datos a comparar

- **Lado MAT:** `negociosMAT` = resultado de `clientePrimaryAPI.ObtenerNegocios(fecha)` (ver sección 3).
- **Lado Data Agro:** `negociosDA` = negocios existentes en la base para esa `fecha`, con `TipoNegocioId == AGENTE_DE_COMPRAS` y `EstadoId` en (`Confirmado`, `Finalizado`).

### 4.2 Agrupamiento

Ambas listas (`negociosMAT` y `negociosDA`) se agrupan por la combinación de:

- `MaterialId`
- `Posicion`
- `OperadorId`
- `MonedaId`
- `DolarExportador`

Por cada grupo se calcula un **precio ponderado** (`PrecioPonderado`): suma de `Precio * |Cantidad|` dividido la suma de `|Cantidad|`.

### 4.3 Campos efectivamente comparados

Para cada combinación de `MaterialId + MonedaId + Posicion + Operador (+ DolarExportador)` se compara:

1. **Precio ponderado** (`PrecioPonderado`, redondeado con formato `"N"` cultura `es-AR`): si el valor de Data Agro difiere del valor del MAT, se agrega un error de "Diferencia de precios ponderados...".
2. **Cantidad total en kilos** (suma de `Cantidad`, tomando valor absoluto según corresponda): si la cantidad en Data Agro difiere de la cantidad en el MAT, se agrega un error de "Diferencia en los kilos totales...".
3. **Existencia del grupo en ambos lados**:
   - Si un grupo existe en el MAT pero no en Data Agro → error "No se encontraron en Data Agro negocios de...".
   - Si un grupo existe en Data Agro pero no en el MAT → error "No se encontraron en el MAT negocios de...".

La comparación se hace primero recorriendo `listaMAT` contra `listaDA`, y luego `listaDA` contra `listaMAT`, para detectar diferencias en ambos sentidos (faltantes de un lado y del otro).

### 4.4 Resultado de la comparación → envío de mail

Una vez comparados todos los grupos:

1. Los negocios existentes en Data Agro (`negociosDA`) se marcan como `EstadoId = 8` (Eliminado).
2. Los negocios nuevos traídos del MAT (`negociosMAT`) se agregan a la base (`repositorio.Agregar`).
3. Se persisten los cambios (`repositorio.GuardarCambios()`).
4. Según el resultado de la lista `errores`:
   - **Si `errores.Count == 0`:** se envía el mail con asunto `"No hay diferencias entre Data Agro y posición MATBA - {fecha}"`, agregando el mensaje "No se encontraron diferencias entre Data Agro y posición MATBA."
   - **Si `errores.Count > 0`:** se envía el mail con asunto `"Error - Diferencias entre Data Agro y posición MATBA - {fecha}"`, incluyendo el detalle de cada diferencia encontrada (precio ponderado, cantidad, o faltantes de un lado u otro).
   - Si ocurre una excepción durante todo el proceso, se envía un mail con asunto `"Error al obtener la posición MATBA - {fecha}"` indicando que no se pudo procesar la sincronización.

### 4.5 Envío del mail (`EnviarMailMATPrimay`)

- Destinatarios: usuarios (`Comercial`) que tengan el permiso `PermisosDataAgro.MailMATPrimary`.
- Copia (CC): dirección configurada en `EmailSoporte` (app settings).
- Cuerpo: encabezado fijo explicando que es el resultado de la comparación automática entre Data Agro y el MAT, seguido de cada mensaje de la lista `errores` (o el mensaje de "sin diferencias").

## 5. Resumen: ¿qué define el mail de "diferencias" o "sin diferencias"?

El mail de **error/diferencias** se dispara si, al agrupar los negocios de Data Agro y del MAT por **Material + Moneda + Posición + Operador + Dólar Exportador**, para algún grupo:

- El **precio ponderado** no coincide entre ambos sistemas, y/o
- La **cantidad total (Kg)** no coincide entre ambos sistemas, y/o
- El grupo existe en uno de los dos sistemas y no en el otro.

Si ninguna de estas condiciones se cumple para ningún grupo, se envía el mail de **"No hay diferencias entre Data Agro y posición MATBA"**.

---

## Versionado

| Versión | Fecha       | Cambios               |
|---------|-------------|-----------------------|
| 1.0     | 29/07/2026  | Documentación inicial |
