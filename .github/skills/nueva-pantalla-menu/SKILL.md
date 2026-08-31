---
name: nueva-pantalla-menu
description: 'Workflow para agregar el link de una pantalla nueva al menú de navegación de WebDataAgro (_Layout.cshtml), protegido por permiso. Usar cuando pidan agregar una pantalla/opción/link nuevo al menú principal.'
---

# Nueva pantalla en el menú

## Patrón (confirmado en `WebDataAgro/Views/Shared/_Layout.cshtml`)

Cada item de menú está protegido por un `@if (PermisosHelper.Is(PermisosDataAgro.{Permiso}))`, dentro del `<ul class="dropdown-menu">` de la sección correspondiente (Reportes, Configuración, Cupos, etc.):

```cshtml
@if (PermisosHelper.Is(PermisosDataAgro.{Permiso}))
{
    <li>
        <a href="/{Controller}/{Accion}" class="nav-link misreporteslink">
            <i class="icon-bar-chart font-color"></i> {Texto del link}
        </a>
    </li>
}
```

## Procedimiento
1. Confirmar con el usuario a qué sección del menú pertenece la pantalla nueva (Reportes / Configuración / Cupos / etc.) para ubicar el `<ul class="dropdown-menu">` correcto en `_Layout.cshtml`.
2. Agregar el `<li>` siguiendo el patrón de arriba, con el permiso (`PermisosDataAgro.{Permiso}`) que corresponda.
3. La sección completa también suele estar envuelta en un `@if` que agrupa **todos** los permisos de esa sección (para mostrar/ocultar el dropdown entero). Si el permiso nuevo no está en esa lista agregada, el item nunca se va a mostrar aunque el usuario tenga el permiso — agregarlo también ahí.

## Fuera de alcance
- Si el permiso (`PermisosDataAgro.{Permiso}`) todavía no existe, avisar al usuario: agregarlo al enum `Molinos.DataAgro.Entities/Seguridad/PermisosDataAgro.cs` (con un valor numérico libre) es un paso aparte, y además puede requerir asignarlo a roles existentes (fuera del alcance de este skill).
