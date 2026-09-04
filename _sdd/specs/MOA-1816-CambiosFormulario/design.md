# Technical Design - MOA-1816: Cambios en formulario

> Base de referencia: el adjunto `distribuidor_cupos_v2.html` del issue y la implementación actual de la solución en `WebDataAgro/Views/Distribucion/`.

## 1. Objetivo funcional

Aplicar en la solución real los cambios definidos por el usuario en el adjunto v2:

1. quitar la exclusión de contratos vencidos;
2. usar `Fecha Hasta Contra` como criterio de rango con fallback a `Fecha Hasta`;
3. mantener la restricción de contratos aún no iniciados (`Fecha Desde > hoy`);
4. no tocar la prioridad de ordenamiento ni el diagnóstico visual basado en `Fecha Hasta`.

## 2. Mapa de impacto

### Frontend / formulario actual

- El ajuste se aplica en la lógica de elegibilidad del formulario real, ubicado en `WebDataAgro/Views/Distribucion/Index.cshtml` y su JS asociado `WebDataAgro/Scripts/Distribucion/distribucion-cupos.js`.
- El cambio puede requerir corregir el cálculo del filtro de vigencia, el diagnóstico y la unión de rangos sobre fechas.
- Si la misma regla se replica en backend o en un parser del archivo, debe mantenerse consistente con el comportamiento del form.

### Reuso de la referencia madre

La tarea sigue dentro del alcance de `MOA-1765-DistribuidorCupos`, pero con la diferencia de que el cambio de negocio se toma como regla de corrección puntual sobre el formulario enviado en el adjunto v2, no como refactor general del flujo.

## 3. Regla de negocio a modificar

### Lógica anterior (actual)

- Se excluía a los contratos con `Fecha Hasta` anterior a hoy.
- El rango de vigencia se evaluaba con `Fecha Hasta` directo.

### Nueva regla

- `if (fd > today) continue;` permanece como validación de contratos aún no iniciados.
- `if (fh && fh < today) continue;` se elimina.
- El rango de evaluación usa `fhc || fh` donde `fhc` representa `Fecha Hasta Contra`.
- El ordenamiento y el diagnóstico siguen usando `Fecha Hasta` tal como lo indica la descripción del issue.

## 4. Decisión de implementación

### Opción recomendada

Aplicar la corrección en la lógica del formulario actual de la solución (JS de la vista), porque es el origen funcional visible del ajuste y coincide con el modelo entregado por el adjunto v2.

### Opción de respaldo

Si la lógica del filtro se reutiliza en backend para validación server-side o APIs, reproducción del mismo cambio en el parser/manager correspondiente para mantener consistencia entre UI y servidor.

## 5. Impacto en la UI y el cálculo

### Elegibilidad

La lista de contratos elegibles debe incluir contratos vencidos que aun así no hayan iniciado (`fd <= today`), por lo que se recuperan los registros que antes quedaban fuera por el criterio `fh < today`.

### Ordenamiento

No debe cambiarse el desempate principal ni el criterio de prioridad actual. La corrección es de rango elegible, no de prioridad entre contratos.

### Diagnóstico

El panel de diagnóstico debe continuar mostrando `Fecha Hasta` como rango visible, aunque la elegibilidad se base en `Fecha Hasta Contra` cuando exista.

## 6. Validación sugerida

- Caso base: 1.265 contratos CTO San Lorenzo, hoy `06.05.2026`.
- Confirmar que la cantidad de elegibles pasa de 486 a 608.
- Validar que la diferencia real se da en 170 filas donde `Fecha Hasta` y `Fecha Hasta Contra` no coinciden.
- Confirmar que los contratos con `Fecha Desde` futura siguen excluidos.
- Confirmar que el painel de diagnóstico y el ordenamiento no cambian más allá de lo pedido.

## 7. Riesgos y consideraciones

- En filas con `Fecha Hasta Contra` vacía, debe aplicarse fallback a `Fecha Hasta`.
- El ajuste debe hacerse sin romper el formato de salida ni la UX del formulario actual.
- La corrección debe ser trazable al adjunto v2 del Jira, no a una interpretación aislada del requisito histórico.
