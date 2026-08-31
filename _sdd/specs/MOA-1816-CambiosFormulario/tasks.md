# Tasks Checklist - MOA-1816: Cambios en formulario

## Fase 1: Revisión del adjunto de Jira y ubicación real

- [ ] Revisar el adjunto `distribuidor_cupos_v2.html` como fuente de verdad del cambio funcional solicitado.
- [ ] Confirmar la implementación actual del formulario en `WebDataAgro/Views/Distribucion/Index.cshtml` y su JS asociado.
- [ ] Ubicar la lógica de filtro de elegibilidad actual (`fd`, `fh`, `fhc`, `today`) y el panel de diagnóstico asociado.
- [ ] Comparar el comportamiento actual contra la regla pedida por el ticket y definir la corrección exacta.

## Fase 2: Ajuste de regla de negocio

- [ ] Eliminar la restricción de vencidos (`if (fh && fh < today) continue;`).
- [ ] Mantener la restricción de contratos no iniciados (`fd > today`).
- [ ] Incorporar `Fecha Hasta Contra` como criterio principal con fallback a `Fecha Hasta` cuando venga vacío.
- [ ] Validar que la corrección se aplica sin cambiar la prioridad de ordenamiento ni el diagnóstico visual.

## Fase 3: Adaptación a la solución actual

- [ ] Modificar la lógica de UI/JS del formulario actual de la solución en `WebDataAgro/Scripts/Distribucion/distribucion-cupos.js`.
- [ ] Ajustar el markup o plantillas de la vista si fuese necesario para reflejar el rango nuevo o una etiqueta de diagnóstico.
- [ ] Repetir la validación en el componente que se usa en producción para que el cambio no quede solo en el POC.
- [ ] Si existe lógica duplicada en backend/parseador, replicar la misma regla allí para mantener consistencia.

## Fase 4: Verificación y QA

- [ ] Ejecutar validación con el archivo de ejemplo del cliente y comparar el volumen de elegibles antes/después.
- [ ] Confirmar que los 122 contratos vencidos vuelven a ser elegibles.
- [ ] Revisar el orden de resultados y el panel de diagnóstico para asegurar que no haya regresión.
- [ ] Revisar que el cambio no convierte `Fecha Hasta Contra` en criterio de ordenamiento si no fue pedido.

## Criterios de cierre de la subtarea

- [ ] La regla de vencidos queda eliminada exactamente como pide el ticket.
- [ ] `Fecha Hasta Contra` queda integrada con fallback a `Fecha Hasta`.
- [ ] El formulario actual de la solución queda alineado con el adjunto v2 del issue.
- [ ] El diagnóstico y el sort con `Fecha Hasta` siguen intactos.
