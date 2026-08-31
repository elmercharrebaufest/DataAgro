---
description: "Convención de subida y validación de archivos en Controllers de WebDataAgro (Excel, KMZ, etc). Use when: agregar o modificar un endpoint que recibe un archivo subido por el usuario (upload de Excel, KMZ, adjuntos)."
---

# Subida de archivos en Controllers

## Patrón existente (ver `CompraNetController.AltaMasivaContratosExcel`)
```csharp
[HttpPost]
public ActionResult {Accion}(/* otros parámetros */)
{
    if (Request.Files.Count == 0) { /* error: debe seleccionar archivo */ }
    if (Request.Files.Count > 1) { /* error: debe seleccionar un solo archivo */ }

    var fileSubido = Request.Files[0];
    var extension = Path.GetExtension(fileSubido.FileName).ToUpper();
    if (extension != ".XLSX") { /* error: formato no soportado */ }

    if (fileSubido.ContentLength > 0)
    {
        var datos = ExcelImport.LeerExcelDesdeHttpRequest(Request);
        // delegar el procesamiento al Manager correspondiente
    }
}
```
- El archivo se accede vía `Request.Files[0]` (no como parámetro `HttpPostedFileBase` de la acción).
- Validación existente: cantidad de archivos, extensión (whitelist simple por `Path.GetExtension`), y `ContentLength > 0`.
- El parseo de Excel se delega a un Helper (`ExcelImport`), no se hace inline en el Controller.

## Recomendación adicional (no es la convención actual, es una mejora a proponer)
El patrón existente valida solo por extensión, no por tamaño máximo ni contenido real del archivo. Si el usuario lo pide o el endpoint es sensible, sugerir agregar:
- Límite explícito de tamaño (`fileSubido.ContentLength` contra un máximo razonable), además de lo que ya limite `Web.config` (`maxRequestLength`/`maxAllowedContentLength`).
- Aclarar que la validación por extensión no garantiza el contenido real del archivo (un `.xlsx` renombrado podría no serlo); si el riesgo lo amerita, proponerlo como mejora explícita al usuario en vez de asumir que ya está cubierto.
