---
description: "Funciones estándar para mostrar mensajes/alertas/confirmaciones al usuario en el JS de WebDataAgro (sobre BootstrapDialog). Use when: mostrar un mensaje de error, éxito, advertencia o pedir confirmación al usuario desde JavaScript en una vista de WebDataAgro."
---

# Notificaciones al usuario (JS)

Todas centralizadas en `WebDataAgro/Scripts/Mastersoft.js`, sobre `BootstrapDialog`. **No usar `alert()` nativo ni agregar librerías nuevas** (toastr, SweetAlert, etc.): reutilizar estas funciones.

| Función | Uso |
|---|---|
| `MensErr(mensaje)` | Mensaje de error |
| `MensInfo(mensaje)` | Mensaje informativo |
| `MensInfoReload(mensaje)` | Mensaje informativo que recarga la página al cerrarse |
| `MensAlerta(mensaje)` | Advertencia |
| `Confirma(mensaje, fncallback)` | Confirmación con Aceptar/Cancelar; `fncallback` se ejecuta solo si el usuario acepta |
| `ConfirmaConAdvertencia(mensaje, fncallback)` | Igual que `Confirma`, con estilo de advertencia |
| `ShowTooltipMessages(prefix, arrayDeErrores)` | Errores de validación por campo (tooltips) |

Ejemplo:
```javascript
Confirma("¿Confirma la anulación del contrato?", function () {
    MSExecuteURLOnServerAsync('/Contrato/Anular?id=' + id, function (data) {
        MensInfo("Contrato anulado correctamente.");
    });
});
```
