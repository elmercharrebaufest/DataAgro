# Requirements - MOA-1816: Cambios en formulario

## 1. Contexto

Esta subtarea ajusta la lógica del formulario de distribución de cupos a partir del adjunto de Jira `distribuidor_cupos_v2.html` y del formulario actual de la solución en `WebDataAgro/Views/Distribucion/Index.cshtml`.

La regla de negocio nueva viene del ticket y reemplaza la lógica anterior del POC base:
- ya no se excluyen contratos vencidos;
- el rango de vigencia usa `Fecha Hasta Contra` con fallback a `Fecha Hasta`;
- sigue vigente la exclusión de contratos que aún no iniciaron (`Fecha Desde > hoy`);
- no se cambia el criterio de desempate ni el diagnóstico visual.

- Jira: MOA-1816 — Cambios formulario
- Fuente directa: adjunto `distribuidor_cupos_v2.html` del issue
- Referencia de solución actual: `WebDataAgro/Views/Distribucion/Index.cshtml` y `WebDataAgro/Scripts/Distribucion/distribucion-cupos.js`
- Referencia funcional original: `_sdd/specs/MOA-1765-DistribuidorCupos/`

## 2. Requisitos en formato EARS

- **R1 - Contratos vencidos ya no se excluyen:**
  - **Cuando** el formulario procesa el set de contratos para determinar elegibilidad, **el sistema debe** omitir la condición que descartaba contratos con `Fecha Hasta` anterior a hoy; solo debe seguir excluyendo los contratos cuya `Fecha Desde` aún no comenzó (`Fecha Desde > hoy`).
  - Tipo de verificación: unit/manual

- **R2 - Rango de elegibilidad usa `Fecha Hasta Contra`:**
  - **Cuando** el formulario evalúa si un contrato entra en el rango vigente, **el sistema debe** usar `Fecha Hasta Contra` como criterio principal y, si esa columna viene vacía, aplicar fallback a `Fecha Hasta` para mantener compatibilidad.
  - Tipo de verificación: unit/manual

- **R3 - Se conserva la exclusión de contratos no iniciados:**
  - **Cuando** un contrato tiene `Fecha Desde` posterior a la fecha actual, **el sistema debe** continuar excluyéndolo del cálculo, aunque se haya quitado la exclusión por vencimiento.
  - Tipo de verificación: unit/manual

- **R4 - No se cambia el criterio de desempate del ordenamiento:**
  - **Cuando** el formulario ordena la salida, **el sistema debe** seguir usando `Fecha Hasta` como criterio de prioridad/desempate vigente y no pasar a ordenar por `Fecha Hasta Contra` ni por la fecha original del adjunto.
  - Tipo de verificación: manual

- **R5 - Diagnóstico sigue mostrando el rango original:**
  - **Cuando** el panel de diagnóstico del formulario presenta el rango de fechas, **el sistema debe** seguir mostrando `Fecha Hasta` como referencia diagnóstica, aun cuando la elegibilidad se base en `Fecha Hasta Contra` con fallback.
  - Tipo de verificación: manual

- **R6 - Adaptación al formulario actual de la solución:**
  - **Cuando** se aplique el ajuste al formulario real de la app, **el sistema debe** incluir la corrección en el componente de UI y/o en la lógica de cálculo (JS y/o backend) que corresponda, sin romper la implementación existente de `Distribucion`.
  - Tipo de verificación: manual

## 3. Alcance funcional

- Ajuste de la regla de elegibilidad del formulario.
- Revisión del cálculo del rango de vigencia usando `Fecha Hasta Contra` y fallback a `Fecha Hasta`.
- Preservación de criterios no afectados: ordenamiento y diagnóstico visual.
- Validación del impacto con el ejemplo del archivo San Lorenzo (1.265 CTO, fecha actual 06.05.2026): 486 → 608 contratos elegibles, recuperando 122 vencidos.
- Alineación con la implementación real de la solución en `WebDataAgro/Views/Distribucion/` y `WebDataAgro/Scripts/Distribucion/`.

## 4. Criterios de aceptación

- Los contratos vencidos ya no quedan descartados por la regla `fh < today`.
- La vigencia del contrato usa `Fecha Hasta Contra` con fallback a `Fecha Hasta`.
- La exclusión de contratos que aún no iniciaron se mantiene.
- El ordenamiento y el panel de diagnóstico siguen mostrando `Fecha Hasta` como criterio actual.
- El ajuste puede aplicarse en el formulario actual de la app sin romper la UI ni la lógica existente.
