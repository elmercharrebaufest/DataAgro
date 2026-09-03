# DAT-1264: Al editar un negocio, no carga la cosecha guardada

## Resumen del problema

Al editar un negocio existente en CompraNet, la cosecha/campaña que fue seleccionada y guardada al crearlo se pierde: el formulario vuelve a mostrar la cosecha configurada por defecto para el material (Configuraciones → ABM Materiales) en lugar de la efectivamente guardada.

## Causa raíz

En `CargarDatosEditar(contrato)`, al setear el material del negocio se dispara:

```js
$("#material").data("kendoDropDownList").value(contrato.MaterialId);
$("#material").data("kendoDropDownList").trigger("change");
```

El handler `change` de `#material` llama, sin condición, a `CargarCampaniaPorMaterial($("#material").val())`, que hace dos llamadas AJAX (`ApiCacheManager.getCampana` / `getCampanaActual`) y, cuando responden, sobrescribe el combo `#campanaId` con la cosecha *actual/por defecto* del material.

Más adelante en la misma función se asigna la cosecha real del negocio:

```js
$("#campanaId").data("kendoDropDownList").value(contrato.CampanaId);
```

Como esta asignación es síncrona y la respuesta de `CargarCampaniaPorMaterial` (disparada por el `trigger("change")`) es asíncrona, el callback async llega después y pisa el valor correcto con el default del material.

`CrearContrato.js` (Contrato "A Precio" / "A Fijar") ya tiene una mitigación parcial: agrega un parámetro `campanaIdForzada` a `CargarCampaniaPorMaterial` para poder forzar la cosecha guardada por sobre la default. Esa mitigación **no estaba replicada** en 3 de los otros 4 flujos de negocio que comparten el mismo patrón:

| Archivo | `CargarCampaniaPorMaterial` | `trigger("change")` de `#material` en `CargarDatosEditar` | Asignación directa de `#campanaId` |
|---|---|---|---|
| Fijacion.js | línea 2883 | líneas 3680-3681 | línea 3726 |
| ContratoAcuerdo.js | línea 3124 | líneas 4080-4081 | línea 4120 |
| AFijar.js | línea 2222 | líneas 3106-3107 | línea 3142 |

**Fason.js queda afuera de este bug**: a diferencia de los otros archivos, su `CargarCampaniaPorMaterial` (línea 2740) no usa `ApiCacheManager` sino `MSExecuteOnServer` con `async: false` (ver [Mastersoft.js](../../Scripts/Mastersoft.js)), es decir que es **síncrona/bloqueante**. Por eso, cuando el `change` de `#material` la dispara, termina de ejecutarse y setear el default *antes* de que el código siga con la asignación directa de `contrato.CampanaId` (línea 3615), que la sobreescribe correctamente. No requiere cambios.

### Por qué no se reproduce fácilmente en local/test

`ApiCacheManager` cachea la respuesta de `getCampana`/`getCampanaActual` por `materialId`. Si el material del negocio editado ya tiene la cosecha cacheada en la sesión (por ejemplo, por haberse precargado al iniciar la pantalla, o por haber creado/editado antes otro negocio con el mismo material), el callback se ejecuta en forma síncrona y la asignación posterior "gana", ocultando el bug. Con un material "frío" (sin caché), la falla es reproducible siempre.

## Plan de implementación (✅ implementado)

### Paso 1 — Agregar parámetro `campanaIdForzada` a `CargarCampaniaPorMaterial`

En `Fijacion.js`, `ContratoAcuerdo.js` y `AFijar.js` (Fason.js no lo necesita, ver más arriba), modificar la función para que acepte un segundo parámetro opcional y, cuando venga informado, priorice ese valor sobre la campaña actual del material (mismo patrón que `CrearContrato.js`):

```js
function CargarCampaniaPorMaterial(value, campanaIdForzada) {
    ...
    function updateIfReady() {
        requestsComplete++;
        if (requestsComplete === totalRequests) {
            viewModel.set("CampanaCombo", resultGrano);
            if (campanaIdForzada !== undefined && campanaIdForzada !== null && campanaIdForzada !== "") {
                $("#campanaId").data("kendoDropDownList").value(campanaIdForzada);
            } else if (campanaActualId) {
                $("#campanaId").data("kendoDropDownList").value(campanaActualId);
            }
        }
    }
    ...
}
```

El parámetro es opcional, por lo que no rompe los demás call-sites existentes (`CargarCampaniaPorMaterial("3")`, la llamada del `change` handler sin segundo argumento, etc.).

### Paso 2 — Forzar la cosecha guardada en `CargarDatosEditar`

En cada archivo, agregar una llamada forzada inmediatamente después del `trigger("change")` de `#material`, y reforzar nuevamente junto a la asignación directa de `#campanaId`:

```js
$("#material").data("kendoDropDownList").value(contrato.MaterialId);
$("#material").data("kendoDropDownList").trigger("change");
CargarCampaniaPorMaterial(contrato.MaterialId, contrato.CampanaId); // reforzar cosecha guardada
```

```js
$("#campanaId").data("kendoDropDownList").value(contrato.CampanaId);
CargarCampaniaPorMaterial(contrato.MaterialId, contrato.CampanaId); // reaseguro si el combo aún se poblaba async
```

### Paso 3 — Validar que no queden otros puntos que pisen `#campanaId` durante la carga

Confirmar que no haya, en el resto de `CargarDatosEditar` de cada archivo, otro `trigger("change")` de `#material`/`#tipoId` posterior que dispare de nuevo la carga sin forzar. Ya se verificó que `cargarContratoAFijarSeleccionado` / `InicializarFijacionEdit` y funciones análogas (`InicializarContratoEdit`, `InicializarFasonEdit`, `InicializarAgenteEdit`, `InicializarAcuerdoEdit`) son código muerto: no se invocan desde ninguna vista, que llaman directamente a `CargarDatosEditar(negocioCargado)`.

### Paso 4 — Pruebas manuales

Para cada tipo de negocio (Fijación, Acuerdo, A Fijar):

1. Crear un negocio con un material "frío" (sin caché en la sesión) y una cosecha distinta a la default del material.
2. Editarlo y confirmar que se muestra la cosecha guardada, no la default.
3. Confirmar que el alta de un negocio nuevo sigue autocompletando la cosecha default correctamente.

Pendiente de ejecutar (requiere ambiente con datos de prueba).
