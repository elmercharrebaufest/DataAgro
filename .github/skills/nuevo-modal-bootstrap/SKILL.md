---
name: nuevo-modal-bootstrap
description: 'Patrón de modal Bootstrap (no Kendo Window) usado en WebDataAgro: contenedor, apertura/cierre por jQuery y wiring con AJAX. Usar cuando pidan agregar un modal/popup/ventana emergente en una vista de WebDataAgro.'
---

# Nuevo modal (Bootstrap)

## Patrón

Los modales de WebDataAgro son **Bootstrap Modal** (no Kendo Window). El contenedor va en el `.cshtml` de la vista (o en una vista parcial `_Modal{Nombre}.cshtml`/`Modal{Nombre}.cshtml`):

```html
<div class="modal fade" id="modal{Nombre}" tabindex="-1" role="dialog">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">{Título}</h4>
            </div>
            <div class="modal-body">
                <!-- contenido -->
            </div>
            <div class="modal-footer">
                <button type="button" class="btn" data-dismiss="modal">Cerrar</button>
            </div>
        </div>
    </div>
</div>
```

La apertura/cierre se dispara desde el JS de la feature (`Scripts/App/{Feature}.js`), normalmente dentro de un callback de éxito de AJAX, **no** desde el propio `.cshtml`:

```javascript
function {Accion}() {
    MSExecuteURLOnServerAsync('/{Controller}/{Accion}', function (data) {
        // completar el contenido del modal con "data" antes de mostrarlo
        $("#modal{Nombre}").modal('show');
    });
}
```
Para cerrarlo desde JS: `$("#modal{Nombre}").modal('hide');`.

## Constraints
- No usar Kendo Window para esto: el proyecto usa Bootstrap Modal de forma consistente.
- Si el modal necesita mostrar una lista/grilla adentro con binding de datos (`data-bind="source:..."`), ver la skill [kendo-frontend](../kendo-frontend/SKILL.md): el modal en sí sigue siendo Bootstrap, pero el contenido interno puede usar MVVM/Kendo.
- No olvidar registrar el JS de la feature en el bundle correspondiente (`WebDataAgro/App_Start/BundleConfig.cs`) si es un archivo nuevo.
