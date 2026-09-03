# DAT-1265: Al editar un negocio, no carga la Procedencia y/o Boleto guardado

## Resumen del problema

Al editar un negocio ya cargado en CompraNet, la Procedencia y/o el Boleto que se visualizan pueden diferir de los valores registrados originalmente. Si el usuario guarda la edición sin advertir la diferencia, el sistema puede producir un error de carga o pisar datos válidos.

Reportado por Claudio Ladogana y Fernando Dianda. No se pudo reproducir de forma directa en primera instancia; este documento describe el análisis de causa raíz hecho sobre el código y el plan para corregirlo.

**Estado: ✅ implementado** (Pasos 1 a 5). Pendiente el Paso 6 (pruebas manuales en ambiente con datos de prueba).


## Causa raíz

### 1. Procedencia (compra): `#ProvinciaId` nunca se completa al editar

En `CargarDatosEditar(contrato)` — la función que puebla el formulario al editar un negocio existente — en [CrearContrato.js](../../Scripts/App/CrearContrato.js#L4368):

```js
if (contrato.LocalidadId !== null && contrato.LocalidadId !== "undefined" && contrato.ProvinciaId !== null && contrato.ProvinciaId !== "undefined") {
    $("#LocalidadCrearContrato").val(contrato.Localidad + "(" + contrato.Provincia + ")");
    HabilitarEstablecimiento();
}
```

Solo se asigna el **texto visible** del combo (`#LocalidadCrearContrato`). El campo oculto `#ProvinciaId` nunca se completa en este flujo.

Esto es asimétrico respecto del flujo de alta (selección de proveedor para un negocio nuevo), donde sí se completan ambos campos ([CrearContrato.js líneas 341-348](../../Scripts/App/CrearContrato.js#L341-L348)):

```js
if (compraNet.LocalidadId != null) {
    if (compraNet.LocalidadId != "" && compraNet.ProvinciaId != "") {
        $("#ProvinciaId").val(compraNet.ProvinciaId);
        $("#LocalidadCrearContrato").val(compraNet.Localidad + " (" + compraNet.Provincia + ")");
        ...
```

y del flujo de selección manual de Localidad por autocomplete ([CrearContrato.js línea 5138](../../Scripts/App/CrearContrato.js#L5138)), que también setea `#ProvinciaId`.

**Impacto verificado:** el guardado (`ObtenerDatos()` en [CopiarContrato.js líneas 744-751](../../Scripts/App/CopiarContrato.js#L744-L751)) no depende de `#ProvinciaId`, sino que reconstruye Provincia/Localidad parseando el texto de `#LocalidadCrearContrato` contra `/CompraNet/ObtenerLocalidadId`, por lo que un guardado simple no falla. El problema aparece en todo lo que sí depende de `#ProvinciaId` mientras se está editando, por ejemplo `SeleccionAutomaticaBolsa()` ([CrearContrato.js línea 5864](../../Scripts/App/CrearContrato.js#L5864)):

```js
function SeleccionAutomaticaBolsa() {
    if ($("#tipoId").val() == "2") {
        var destino = $("#destinoId").val();
        var provincia = $("#ProvinciaId").val(); // siempre "" al editar
        var localidadInput = $("#LocalidadCrearContrato").val();
        var bolsa = 0;
        if (destino != "" && provincia != "" && localidadInput != "") {
            bolsa = MSExecuteOnServer('/ConfiguracionBolsa/TraerConfiguracionBolsaConDestinoYProcedencia', { destinoId: destino, provinciaId: provincia });
        }
        ...
```

Como `provincia` siempre es `""` durante la edición, esta función queda inoperante mientras se edita un negocio (no valida/sugiere la Bolsa correcta en base a Destino + Procedencia).

### 2. Procedencia (venta): mismo patrón en `#ProvinciaVentaId`

Para negocios con "Venta" tildada, ocurre lo mismo con el campo de procedencia de venta ([CrearContrato.js líneas 4863-4865](../../Scripts/App/CrearContrato.js#L4863-L4865)):

```js
if (contrato.ProcedenciaVentaId !== null && contrato.ProcedenciaVentaId !== "undefined" && contrato.ProvinciaVentaId !== null && contrato.ProvinciaVentaId !== "undefined") {
    $("#LocalidadVenta").val(contrato.LocalidadVenta + "(" + contrato.ProvinciaVenta + ")");
}
```

Solo se completa `#LocalidadVenta` (texto visible); `#ProvinciaVentaId` nunca se asigna en edición (solo se asigna al seleccionar manualmente una localidad desde el autocomplete, [línea 2867](../../Scripts/App/CrearContrato.js#L2867)). El guardado de Venta tampoco lee `#ProvinciaVentaId` (parsea `#LocalidadVenta` igual que el lado compra), por lo que el guardado simple no se ve afectado, pero el campo queda inconsistente para cualquier lógica futura que lo use.

### 3. Typo relacionado (código muerto, no tocar sin corregir el punto 1 primero)

En el handler `change` de `#tipoId` ([CrearContrato.js línea 897](../../Scripts/App/CrearContrato.js#L897)):

```js
if ($("#provinciaId").val() == "") {   // minúscula: el input real es "#ProvinciaId"
    obtenerLocalidadProvincia();
}
```

El id real del input es `ProvinciaId` (mayúscula, ver [CrearContrato.cshtml línea 646](../../Views/CompraNet/CrearContrato.cshtml#L646)), por lo que el selector nunca matchea y `obtenerLocalidadProvincia()` jamás se ejecuta desde este punto. Es inofensivo *mientras* el punto 1 no esté corregido. **Si se corrige el typo sin corregir el punto 1**, como `#ProvinciaId` real quedaría siempre vacío durante la edición, la condición pasaría a ser siempre verdadera y `obtenerLocalidadProvincia()` **sobrescribiría** la Procedencia guardada con una sugerida en base a proveedor + material + campaña en cada edición. Por eso ambos puntos deben resolverse juntos.

### 4. Boleto (compra): riesgo de pisado silencioso al guardar

La carga de los checkboxes de Boleto en `CargarDatosEditar` ([CrearContrato.js líneas 4487-4515](../../Scripts/App/CrearContrato.js#L4487-L4515)) es correcta si `contrato.BoletoId` llega con un valor 1-5 desde el servidor (confirmado que `ContratoManager.TraerContrato` lo mapea bien desde `Negocio.BoletoId`).

El riesgo está en el guardado ([CopiarContrato.js líneas 688-707](../../Scripts/App/CopiarContrato.js#L688-L707)):

```js
if ($("#boletoConfirmaId").is(':checked')) { obj.BoletoId = 1; obj.BolsaId = $("#bolsaConfirmaId").val(); }
else if ($("#boletoFisicoId").is(':checked')) { obj.BoletoId = 2; ... }
else if ($("#boletoCartaId").is(':checked')) { obj.BoletoId = 4; ... }
else if ($("#boletoNingunoId").is(':checked')) { obj.BoletoId = 3; ... }
else if ($("#sinBoletoId").is(':checked')) { obj.BoletoId = 5; ... }
// si ninguno queda tildado, obj.BoletoId nunca se asigna (undefined)
```

Si por cualquier motivo (dato legacy con `BoletoId` nulo, o timing de carga) ningún checkbox queda tildado al momento de guardar, `obj.BoletoId` queda `undefined` y se omite del JSON enviado al servidor, lo que puede resultar en que el negocio quede guardado sin Boleto (pisando un valor previamente válido) sin ningún error visible para el usuario. Esto coincide con el criterio de aceptación del ticket: *"si el usuario no modifica estos valores, al guardar la edición no debe producirse un error de carga"*.

## Plan de implementación

### Paso 1 — Completar `#ProvinciaId` en `CargarDatosEditar` (lado compra) (✅ implementado)

En [CrearContrato.js](../../Scripts/App/CrearContrato.js#L4368):

```js
if (contrato.LocalidadId !== null && contrato.LocalidadId !== "undefined" && contrato.ProvinciaId !== null && contrato.ProvinciaId !== "undefined") {
    $("#ProvinciaId").val(contrato.ProvinciaId);
    $("#LocalidadCrearContrato").val(contrato.Localidad + "(" + contrato.Provincia + ")");
    HabilitarEstablecimiento();
}
```

### Paso 2 — Completar `#ProvinciaVentaId` en `CargarDatosEditar` (lado venta) (✅ implementado)

En [CrearContrato.js](../../Scripts/App/CrearContrato.js#L4863):

```js
if (contrato.ProcedenciaVentaId !== null && contrato.ProcedenciaVentaId !== "undefined" && contrato.ProvinciaVentaId !== null && contrato.ProvinciaVentaId !== "undefined") {
    $("#ProvinciaVentaId").val(contrato.ProvinciaVentaId);
    $("#LocalidadVenta").val(contrato.LocalidadVenta + "(" + contrato.ProvinciaVenta + ")");
}
```

### Paso 3 — Corregir el typo `#provinciaId` → `#ProvinciaId` (✅ implementado)

En [CrearContrato.js línea 897](../../Scripts/App/CrearContrato.js#L897), corregir el casing **solo después** de aplicar el Paso 1 (para que `obtenerLocalidadProvincia()` no pise la Procedencia recién cargada correctamente):

```js
if ($("#ProvinciaId").val() == "") {
    obtenerLocalidadProvincia();
}
```

Dado que con el Paso 1 aplicado `#ProvinciaId` ya tendrá valor durante la edición, esta condición se mantendrá falsa en ese caso (comportamiento esperado) y seguirá disparándose normalmente para negocios nuevos, que es su propósito original.

### Paso 4 — Endurecer el guardado del Boleto (defensivo) (✅ implementado)

En [CopiarContrato.js](../../Scripts/App/CopiarContrato.js#L688-L710), se agregó un `else` final que deja constancia en consola cuando ningún checkbox de Boleto está tildado al guardar un negocio de tipo Contrato/A Fijar/Contrato Acuerdo, en lugar de omitir `BoletoId` silenciosamente:

```js
else {
    console.warn("Ningún Boleto tildado al guardar: se omite BoletoId (DAT-1265).");
}
```

Se optó por un `console.warn` (no bloqueante) porque no hay evidencia de que "ningún boleto tildado" sea siempre inválido para los tres tipos de negocio cubiertos por esta rama; bloquear el guardado (`MensErr`) requeriría validar primero con QA/negocio si existe algún flujo legítimo sin Boleto tildado. Queda como mejora a evaluar.

### Paso 5 — Otros tipos de negocio (verificado y ✅ implementado)

**Fason y Agente de Compras no requieren cambios propios**: el controlador `CompraNetController.CrearContrato` solo devuelve una vista dedicada (`AFijar`, `Fijacion`, `ContratoAcuerdo`) para los `TipoNegocioId` A_FIJAR, FIJACION y CONTRATO_ACUERDO; para cualquier otro tipo (incluidos Fason y Agente de Compras) devuelve siempre la vista `CrearContrato`:

```csharp
return View(tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || tipoNegocio.TipoNegocioId == (int)EnumTipoNegocio.CONTRATO_ACUERDO ? tipoNegocio.Descripcion.Replace(" ", String.Empty) : "CrearContrato");
```

Es decir que Fason y Agente de Compras editan usando `CrearContrato.js`, el mismo archivo ya cubierto en los Pasos 1-3. Existen `Fason.cshtml` (que referencia un bundle `~/bundles/CompraNetFason` inexistente en `BundleConfig.cs`) y `Fason.js` con su propio `CargarDatosEditar`, pero esa vista **nunca se devuelve** desde el controlador (no hay ningún `return View("Fason")` ni tipo mapeado a ella) y el bundle que referencia no está registrado: es código muerto/inalcanzable, no requiere corrección. No existe un archivo `Agente.js` separado.

**Fijación (`Fijacion.js`), A Fijar (`AFijar.js`) y Contrato Acuerdo (`ContratoAcuerdo.js`) sí tienen el mismo bug**, porque cada uno define su propia copia de `CargarDatosEditar` con el código idéntico al de `CrearContrato.js`:

| Archivo | Procedencia compra (`#ProvinciaId`) | Procedencia venta (`#ProvinciaVentaId`) | Typo `#provinciaId` (código muerto) |
|---|---|---|---|
| `AFijar.js` | línea 3141-3145 | no aplica (no maneja Venta) | línea 718 |
| `Fijacion.js` | línea 3725-3729 | no aplica (no maneja Venta) | línea 1056 |
| `ContratoAcuerdo.js` | línea 4119-4123 | línea 4610-4613 | línea 778 |

El guardado (`ObtenerDatos` / `GrabarContrato`) para los cuatro tipos está centralizado en `CopiarContrato.js` (incluido en los 4 bundles), por lo que el Paso 4 (defensivo sobre Boleto) también aplica a los cuatro sin necesidad de duplicarlo.

Se replicaron los mismos cambios de los Pasos 1-3 en los 3 archivos:
- `AFijar.js`: se agregó `$("#ProvinciaId").val(contrato.ProvinciaId);` en el bloque de la línea 3141, y se corrigió el typo de la línea 718.
- `Fijacion.js`: mismo cambio en la línea 3725 y typo de la línea 1056.
- `ContratoAcuerdo.js`: mismo cambio en la línea 4119 (compra) y 4610 (venta, se agregó `$("#ProvinciaVentaId").val(contrato.ProvinciaVentaId);`), y typo de la línea 778.

### Paso 6 — Pruebas manuales

1. Editar un Contrato (tipoId=2) con Procedencia y Boleto guardados. Verificar en DevTools que `#ProvinciaId` (y `#ProvinciaVentaId` si es Venta) quedan completos al abrir el formulario.
2. Cambiar el Destino sin tocar Procedencia/Boleto y confirmar que `SeleccionAutomaticaBolsa()` ahora evalúa correctamente la Bolsa sugerida.
3. Guardar sin modificar nada y confirmar en base (`Negocio.ProvinciaId`, `LocalidadId`, `BoletoId`, `BolsaId`, `ProcedenciaVentaId`) que los valores no cambian.
4. Confirmar que el alta de un negocio nuevo sigue autocompletando Procedencia por defecto según proveedor/material/campaña (no debe romperse por el Paso 3).
5. Repetir los puntos 1-3 para A Fijar, Fijación y Contrato Acuerdo (los tres comparten el bug, ver Paso 5). Fason y Agente de Compras no requieren prueba adicional porque reutilizan `CrearContrato.js`.

Pendiente de ejecutar (requiere ambiente con datos de prueba y confirmación con QA/reporters del ticket).
