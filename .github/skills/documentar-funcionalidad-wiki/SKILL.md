---
name: documentar-funcionalidad-wiki
description: 'Workflow para generar o actualizar la documentación funcional de una modificación de código o una nueva funcionalidad en la carpeta llamada "_DocumentacionCopilot", manteniendo un historial de versiones dentro del mismo documento. Usar cuando pidan documentar una feature/cambio, actualizar la documentación funcional de un módulo, o dejar registro de un cambio con versionado.'
---

# Documentar funcionalidad en "_DocumentacionCopilot"

## Cuándo usar
- Al terminar de implementar o modificar una funcionalidad y haya que dejar registro funcional en "_DocumentacionCopilot".
- Cuando pidan "documentá/actualizá X en documentación copilot" o "generá el changelog de esta página".

## Procedimiento

1. **Identificar la página a documentar**
   - Buscar si ya existe un archivo (por nombre del módulo/feature/archivo tocado).
   - Si existe, traer el contenido completo para conservar el historial previo.
   - Si no existe, proponer un path nuevo bajo `/_DocumentacionCopilot/{Nombre del módulo o feature}` (ej. `/_DocumentacionCopilot/Auditoria Contacto Comercial`), y confirmar el nombre con el usuario antes de crearla.

2. **Recolectar el contenido funcional**
   - Basarse en los cambios reales: diff/commits de la rama actual, o lo conversado en la sesión. No inventar alcance no confirmado.
   - Cubrir como mínimo: qué problema resuelve o qué hace la funcionalidad, disparadores (Job/Controller/acción que la inicia), Managers/Agents/archivos clave involucrados, reglas de negocio relevantes, y cómo probarla si aplica.

3. **Aplicar la plantilla y el versionado** (ver [plantilla](./assets/template.md))
   - Encabezado con `**Versión:**`, `**Última actualización:**` (fecha de hoy, `DD/MM/AAAA`) y `**Autor:**`.
   - Cuerpo del documento con las secciones funcionales, numeradas.
   - Sección final `## Historial de versiones` con una tabla que **nunca se borra ni reescribe**: solo se le agrega una fila nueva por cada actualización.
     - Columnas: `Versión | Fecha | Autor | Descripción del cambio`.
   - Reglas de versionado (semántica simple, dos dígitos):
     - **Página nueva** → Versión `1.0`.
     - **Cambio menor** (ajuste, aclaración, corrección puntual, sección chica agregada) → incrementar el segundo dígito (`1.0` → `1.1`).
     - **Cambio mayor** (rediseño del flujo documentado, nueva funcionalidad grande dentro del mismo documento) → incrementar el primer dígito y resetear el segundo (`1.1` → `2.0`).
     - Si el alcance del cambio es ambiguo, preguntarle al usuario si es menor o mayor en vez de asumir.
   - Actualizar `**Versión:**` y `**Última actualización:**` del encabezado a los nuevos valores, y agregar la fila correspondiente al historial.

4. **Guardar el archivo**
   - Guardar el archivo en la ubicación correspondiente bajo `/_DocumentacionCopilot/`.

5. **Confirmar**
   - Informar al usuario el path final y la versión asignada.

## Fuera de alcance
- No borra ni reescribe filas previas del historial de versiones.
- No genera documentación técnica de código (comentarios, README de repo): esto es documentación **funcional**.
- No decide si un cambio es menor o mayor cuando es ambiguo: pregunta al usuario.
