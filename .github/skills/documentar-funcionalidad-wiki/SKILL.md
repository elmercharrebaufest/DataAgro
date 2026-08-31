---
name: documentar-funcionalidad-wiki
description: 'Workflow para generar o actualizar la documentación funcional de una modificación de código o una nueva funcionalidad en la Wiki de DataAgro (Azure DevOps, wiki `DataAgro.wiki`), manteniendo un historial de versiones dentro del mismo documento. Usar cuando pidan documentar una feature/cambio en la wiki, actualizar la documentación funcional de un módulo, o dejar registro de un cambio con versionado en Azure DevOps.'
---

# Documentar funcionalidad en la Wiki de DataAgro

## Cuándo usar
- Al terminar de implementar o modificar una funcionalidad y haya que dejar registro funcional en la Wiki de Azure DevOps (organización `molinosagro`, proyecto `DataAgro`, wiki `DataAgro.wiki`).
- Cuando pidan "documentá/actualizá X en la wiki" o "generá el changelog de esta página".

## Prerrequisito
Requiere el servidor MCP de Azure DevOps (`mcp_azuredevops_m_wiki*`, `mcp_azuredevops_m_search_wiki`) activo con acceso a `molinosagro`/`DataAgro`. Si esas tools no aparecen disponibles, avisar al usuario que inicie el servidor MCP (`.vscode/mcp.json` → `ado-remote-mcp`) antes de continuar. No inventar contenido de wiki que no se pudo leer.

## Procedimiento

1. **Identificar la página a documentar**
   - Buscar si ya existe una página relacionada con `search_wiki` (por nombre del módulo/feature/archivo tocado) o listando con `wiki` acción `list_pages` (`wikiIdentifier: DataAgro.wiki`, `project: DataAgro`).
   - Si existe, traer el contenido completo con `wiki` acción `get_page` (`includeContent: true`) para conservar el historial previo y el `eTag` (necesario para el guardado).
   - Si no existe, proponer un path nuevo bajo `/DataAgro/{Nombre del módulo o feature}` (ej. `/DataAgro/Auditoria Contacto Comercial`), siguiendo la convención de la página existente `/DataAgro/Primary API (MATBA)`, y confirmar el nombre con el usuario antes de crearla.

2. **Recolectar el contenido funcional**
   - Basarse en los cambios reales: diff/commits de la rama actual, o lo conversado en la sesión. No inventar alcance no confirmado.
   - Cubrir como mínimo: qué problema resuelve o qué hace la funcionalidad, disparadores (Job/Controller/acción que la inicia), Managers/Agents/archivos clave involucrados, reglas de negocio relevantes, y cómo probarla si aplica.

3. **Aplicar la plantilla y el versionado** (ver [plantilla](./assets/template.md))
   - Encabezado con `**Versión:**`, `**Última actualización:**` (fecha de hoy, `DD/MM/AAAA`) y `**Autor:**` — mismo formato que usa ya la página `Primary API (MATBA)`.
   - Cuerpo del documento con las secciones funcionales, numeradas.
   - Sección final `## Historial de versiones` con una tabla que **nunca se borra ni reescribe**: solo se le agrega una fila nueva por cada actualización.
     - Columnas: `Versión | Fecha | Autor | Descripción del cambio`.
   - Reglas de versionado (semántica simple, dos dígitos):
     - **Página nueva** → Versión `1.0`.
     - **Cambio menor** (ajuste, aclaración, corrección puntual, sección chica agregada) → incrementar el segundo dígito (`1.0` → `1.1`).
     - **Cambio mayor** (rediseño del flujo documentado, nueva funcionalidad grande dentro del mismo documento) → incrementar el primer dígito y resetear el segundo (`1.1` → `2.0`).
     - Si el alcance del cambio es ambiguo, preguntarle al usuario si es menor o mayor en vez de asumir.
   - Actualizar `**Versión:**` y `**Última actualización:**` del encabezado a los nuevos valores, y agregar la fila correspondiente al historial.

4. **Guardar la página**
   - Usar `wiki_upsert_page` con `wikiIdentifier: DataAgro.wiki`, `project: DataAgro`, `path`, `content` (el documento completo actualizado, no un diff parcial) y el `etag` obtenido en el paso 1 si la página ya existía (evita sobrescribir cambios concurrentes de otra persona).
   - Si el `etag` quedó desactualizado (conflicto de la API), volver a traer la página con `get_page` y reintentar con el `etag` nuevo.

5. **Confirmar**
   - Informar al usuario el path final, la versión asignada, y el `remoteUrl` de la página devuelto por la API.

## Fuera de alcance
- No borra ni reescribe filas previas del historial de versiones.
- No genera documentación técnica de código (comentarios, README de repo): esto es documentación **funcional** para la wiki.
- No decide si un cambio es menor o mayor cuando es ambiguo: pregunta al usuario.
