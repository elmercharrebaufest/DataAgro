# Tasks Checklist - MOA-1816: Cambios en formulario

## Fase 1: Revisión del adjunto de Jira y ubicación real

- [x] Revisar el adjunto `distribuidor_cupos_v2.html` como fuente de verdad del cambio funcional solicitado.
- [x] Confirmar la implementación actual del formulario en `WebDataAgro/Views/Distribucion/Index.cshtml` y su JS asociado.
- [x] Ubicar la lógica de filtro de elegibilidad actual (`fd`, `fh`, `fhc`, `today`) y el panel de diagnóstico asociado.
- [x] Comparar el comportamiento actual contra la regla pedida por el ticket y definir la corrección exacta.

## Fase 2: Ajuste de regla de negocio

- [x] Eliminar la restricción de vencidos (`if (fh && fh < today) continue;`).
- [x] Mantener la restricción de contratos no iniciados (`fd > today`).
- [x] Incorporar `Fecha Hasta Contra` como criterio principal con fallback a `Fecha Hasta` cuando venga vacío.
- [x] Validar que la corrección se aplica sin cambiar la prioridad de ordenamiento (test unitario
      `Calcular_OrdenamientoUsaFechaHastaYNoFechaHastaContra`); el diagnóstico visual queda
      pendiente de confirmación manual (ver `qaManualPending` R5 en `feature.json`).

## Fase 3: Adaptación a la solución actual

- [ ] Modificar la lógica de UI/JS del formulario actual de la solución en `WebDataAgro/Scripts/Distribucion/distribucion-cupos.js`.
      **Nota:** no aplicó — la elegibilidad se calcula 100% server-side (`SapArchivoParserManager`
      y `DistribucionCuposManager`); el JS de la vista no tiene lógica de filtro por fechas, así que
      no había nada que modificar ahí. Ver "Opción de respaldo" en `design.md`.
- [ ] Ajustar el markup o plantillas de la vista si fuese necesario para reflejar el rango nuevo o una etiqueta de diagnóstico.
      **Nota:** no fue necesario — R5 pide explícitamente no tocar el panel de diagnóstico.
- [x] Repetir la validación en el componente que se usa en producción para que el cambio no quede solo en el POC
      (los cambios se aplicaron directamente en los Managers de producción, no en un POC aparte).
- [x] Si existe lógica duplicada en backend/parseador, replicar la misma regla allí para mantener consistencia
      (`SapArchivoParserManager` y `DistribucionCuposManager` quedaron alineados).

## Fase 4: Verificación y QA

- [ ] Ejecutar validación con el archivo de ejemplo del cliente y comparar el volumen de elegibles antes/después.
      **Pendiente de QA manual** (ver `feature.json` → `qaManualPending` R1/R2).
- [ ] Confirmar que los 122 contratos vencidos vuelven a ser elegibles.
      **Pendiente de QA manual** con el archivo real de San Lorenzo.
- [ ] Revisar el orden de resultados y el panel de diagnóstico para asegurar que no haya regresión.
      **Parcial:** el orden ya tiene test unitario en verde; el panel de diagnóstico sigue
      pendiente de confirmación visual manual (ver R5).
- [x] Revisar que el cambio no convierte `Fecha Hasta Contra` en criterio de ordenamiento si no fue pedido
      (test unitario `Calcular_OrdenamientoUsaFechaHastaYNoFechaHastaContra` en verde).

## Criterios de cierre de la subtarea

- [x] La regla de vencidos queda eliminada exactamente como pide el ticket (test unitario en verde).
- [x] `Fecha Hasta Contra` queda integrada con fallback a `Fecha Hasta` (test unitario en verde).
- [ ] El formulario actual de la solución queda alineado con el adjunto v2 del issue.
      **Pendiente de QA manual** — la lógica de negocio está alineada y probada; falta la validación
      visual/funcional en navegador (R6).
- [ ] El diagnóstico y el sort con `Fecha Hasta` siguen intactos.
      **Parcial:** el sort tiene test unitario en verde; el diagnóstico visual sigue pendiente de
      confirmación manual (R5).

