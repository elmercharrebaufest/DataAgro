---
name: nuevo-reporte-exportable
description: 'Workflow para agregar la capa de datos/wiring de un reporte exportable (PDF/Excel) en DataAgro, asumiendo que el diseño visual del reporte (ActiveReports) ya existe. Usar cuando pidan agregar/exportar un nuevo reporte, listado descargable, PDF o Excel desde una pantalla de WebDataAgro.'
---

# Nuevo reporte exportable (capa de datos/wiring)

## Alcance y límite importante
Los reportes de DataAgro tienen **dos capas separadas**:
- **Capa visual** (`Molinos.DataAgro.Report/Reportes/Rpt{X}.cs` + `.Designer.cs` + `.resx`): diseñada con el **designer visual de ActiveReports en Visual Studio**. Esta skill **no la genera**: si el reporte visual todavía no existe, avisar al usuario que primero debe crearse (o clonarse de uno existente) con el designer gráfico, antes de continuar.
- **Capa de datos/wiring** (`Molinos.DataAgro.Report/Clases/Lst{X}.cs`): esto sí es código y es lo que esta skill arma.

## Procedimiento (asumiendo que `Rpt{X}` ya existe)

1. **Crear la clase `Lst{X}`** en `Molinos.DataAgro.Report/Clases/Lst{X}.cs`:
   ```csharp
   public class Lst{X}
   {
       private readonly IReportesManager reportesManager;

       public Lst{X}(IReportesManager reportesManager)
       {
           this.reportesManager = reportesManager;
       }

       public string GenerarListado(List<{Dato}> datos)
       {
           var reporte = new Rpt{X}();
           reporte.DataSource = datos;
           reporte.Run(false);

           var export = new PdfExport(); // o el export de Excel con OfficeOpenXml si corresponde
           var identif = Varios.GetIdentif();

           using (var ms = new MemoryStream())
           {
               export.Export(reporte.Document, ms);
               reportesManager.GrabarReporte(new Reportes
               {
                   Identificador = identif,
                   FileName = "{X}.pdf",
                   Contenido = ms.ToArray().ReplaceText()
               });
           }
           return identif;
       }
   }
   ```

2. **(Si hace falta traer los datos) Agregar el método en `I{X}Manager`/`{X}Manager`** que arma la lista de `{Dato}` a exportar (ver skill [nueva-operacion-manager](../nueva-operacion-manager/SKILL.md) si es una consulta nueva).

3. **Wiring desde el Controller**: la acción arma los datos, instancia `Lst{X}` con `reportesManager` inyectado, llama `GenerarListado(...)` y devuelve la clave de descarga con `Util.GetDownloadKey(identif)` (servida luego por `DownLoadController`, ya existente).

4. **Disparo de la descarga desde el JS** (`Scripts/App/{Feature}.js`): no es AJAX ni un `<a href>` directo, es un redirect de navegador una vez que el servidor devolvió el `DownloadKey`:
   ```javascript
   function Descargar{X}(param) {
       MSExecuteURLOnServerAsync('/{Controller}/{Accion}', function (data) {
           if (data.DownloadKey && data.DownloadKey.length > 0) {
               window.location = MSGetUrl('/DownLoad/{Formato}?key=' + data.DownloadKey);
           }
       });
   }
   ```

## Constraints
- No intentar crear ni modificar `Rpt{X}.cs`/`.Designer.cs`/`.resx`: son archivos generados por el designer visual, no se editan a mano de forma confiable.
- Si el reporte visual no existe todavía, decirlo explícitamente y detenerse ahí en vez de improvisar un reemplazo.
