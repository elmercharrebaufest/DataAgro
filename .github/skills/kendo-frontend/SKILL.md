---
name: kendo-frontend
description: 'Patrones de uso de Kendo UI (JS/jQuery) en el front de WebDataAgro: grids (client-only y server-driven), DropDownList, AutoComplete, DatePicker y NumericTextBox. Usar cuando pidan agregar o modificar una grilla, un combo, un buscador con autocompletado, un selector de fecha o un campo numérico en una vista de WebDataAgro.'
---

# Kendo UI en WebDataAgro

## Contexto técnico
- No se usa Kendo.Mvc (sin wrappers Razor `@(Html.Kendo()...)`). Todo es JS/jQuery: `$("#elemento").kendoWidget({...})`.
- Cultura siempre en español: `kendo.culture("es-AR")` (fechas `dd/MM/yyyy`, decimales con `,`).
- Convención de archivos: un JS por feature en `WebDataAgro/Scripts/App/{Feature}.js`. Features grandes lo dividen en varios archivos con sufijo (ej. `ControlDeBoletos*.js`).
- **Bundling (paso que se olvida seguido)**: el JS de la feature no se referencia con un `<script src="...">` inline en la vista; se registra como bundle en `WebDataAgro/App_Start/BundleConfig.cs` (ej. `bundles.Add(new ScriptBundle("~/bundles/{feature}").Include("~/Scripts/App/{Feature}.js"))`) y se renderiza en la vista con `@Scripts.Render("~/bundles/{feature}")`. Si se crea un JS nuevo y no se registra en `BundleConfig.cs`, el archivo queda escrito pero nunca se carga en la página. El `<script src="...">` inline (ej. `KendoExtensions.js`) es la excepción, no la regla.
- Helpers compartidos en `WebDataAgro/Scripts/KendoExtensions.js` (`KendoGridFilterDatePicker`, `KendoGridFilterDropDownList`, `KendoGridFilterAutoComplete`, `KendoGrid_FixFilter`): reutilizarlos para filtros de columna de grid en vez de reinventar el widget inline.

## Grid

### Server-driven (usar por defecto, salvo que el dataset sea chico/estático)

Cruza 4 capas. Flujo completo (ver también la skill [patron-consulta-comando](../patron-consulta-comando/SKILL.md) para el patrón general de Consulta/Comando EF):

1. **Repository** (`Molinos.DataAgro.Repository/ConsultasEF/Traer{X}PorFiltro.cs`): clase `IConsultaEscalar<DataSourceResult>` que arma el `IQueryable` y llama `queryable.ToDataSourceResult<TProyeccion>(request)` (paquete `Kendo.DynamicLinq`).
   ```csharp
   public class Traer{X}PorFiltro : IConsultaEscalar<DataSourceResult>
   {
       private readonly DataSourceRequest request;
       public Traer{X}PorFiltro(DataSourceRequest request) { this.request = request; }

       public virtual DataSourceResult Ejecutar(DbContext contexto)
       {
           var query = contexto.Set<{Entidad}>().Where(...);
           return query.ToDataSourceResult<{Proyeccion}>(request);
       }
   }
   ```
2. **Manager** (`Molinos.DataAgro.Business/Managers/{X}Manager.cs`): método que delega en la Consulta.
   ```csharp
   public DataSourceResult Traer{X}Filtrados(DataSourceRequest filtro)
       => repositorio.ObtenerConsultaEscalar(new Traer{X}PorFiltro(filtro));
   ```
   Agregar también la firma en `I{X}Manager`.
3. **Controller** (`WebDataAgro/Controllers/{X}Controller.cs`): acción `[HttpPost]` que recibe `DataSourceRequest` (bindeado automáticamente por `KendoGridBinder`) y devuelve JSON.
   ```csharp
   [HttpPost]
   public ActionResult Buscar{X}(DataSourceRequest filtro)
   {
       var model = manager.Traer{X}Filtrados(filtro);
       return new JsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
   }
   ```
4. **Vista + JS**: contenedor `<div id="grid{X}"></div>` en la `.cshtml` + inicialización en `Scripts/App/{Feature}.js` con `serverPaging`/`serverSorting`/`serverFiltering: true`.
   ```javascript
   $("#grid{X}").kendoGrid({
       dataSource: {
           type: "aspnetmvc-ajax",
           transport: { read: { url: "/{X}/Buscar{X}", type: "POST" } },
           schema: { data: "Data", total: "Total" },
           serverPaging: true, serverSorting: true, serverFiltering: true,
           pageSize: 20
       },
       sortable: true, pageable: true, filterable: true,
       columns: [ /* ... */ ]
   });
   ```

### Client-only (solo si el dataset es chico o ya viene resuelto)

Ver `WebDataAgro/Scripts/App/ContactoComercialAuditoria.js`: `dataSource: { data: [] }` inicializado vacío, y se carga a mano tras un llamado AJAX con `grid.data("kendoGrid").dataSource.data(resultado)`.

## DropDownList

Combo simple alimentado por una lista chica (vía `ViewBag`/JSON inline o un endpoint que devuelve todos los valores de una vez, no paginado):

```javascript
$("#material").kendoDropDownList({
    optionLabel: "SELECCIONE UN MATERIAL...",
    dataTextField: "Descripcion",
    dataValueField: "MaterialId"
});
```
Patrón para permitir "borrar" la selección con la tecla Supr (visto en varios ABMs):
```javascript
$("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
    if (e.keyCode == 46) { $("#material").data("kendoDropDownList").text(""); }
});
```

## AutoComplete (buscador con datos remotos)

Para buscadores tipo "escribir 3+ letras y traer del servidor" (ej. proveedores, corredores). El Controller expone una acción `ListarX(string text)` que devuelve una lista chica de objetos `{ Id, Filtro }`:

```javascript
$("#buscadorProveedor").kendoAutoComplete({
    template: '<p>#: data.RazonSocial# (#: data.Cuit#)</p>',
    minLength: 3,
    enforceMinLength: true,
    dataTextField: "Filtro",
    dataValueField: "Id",
    filter: "contains",
    dataSource: {
        serverFiltering: true,
        transport: {
            read: { type: "post", dataType: "json", url: "/{X}/BuscarX" },
            parameterMap: function (data) { return { filtro: $("#buscadorProveedor").val() }; }
        }
    }
});
```

## DatePicker y NumericTextBox

```javascript
$("#fechaDesde").kendoDatePicker({ format: "dd/MM/yyyy", parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"] });

$("#Cantidad").kendoNumericTextBox({ culture: "es-AR", format: "n0", spinners: false });
$("#Precio").kendoNumericTextBox({ culture: "es-AR", format: "n2", spinners: false, min: 0 });
```

## Fuera de alcance
- No usar Kendo.Mvc / helpers Razor: el proyecto no los tiene referenciados.
- No forzar un nombre de método nuevo para las acciones server-driven (`Buscar`/`Traer` conviven); seguir el verbo ya usado en el Controller/Manager del mismo dominio.
