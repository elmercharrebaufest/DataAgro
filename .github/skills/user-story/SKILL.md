---
name: user-story
description: Plantillas para historias de usuario, criterios de aceptación, reglas de negocio y análisis de gaps. Usar cuando necesites definir o refinar requerimientos funcionales de negocio en DataAgro.
---

# User Story Toolkit — DataAgro

## Historia de usuario

```text
Como [rol],
quiero [objetivo],
para [beneficio].
```

**Roles reales del dominio** — usar estos, no roles técnicos:

| Rol | Descripción |
|---|---|
| Comercial | Gestiona negocios/contratos, boletos y agenda comercial con proveedores |
| Corredor | Bróker que opera contratos en representación de un grupo de comerciales |
| Administrativo | Controla y concilia boletos/contratos contra SAP, gestiona certificación |
| Administrador del sistema | Configura roles/permisos, campañas, materiales y centros |

**Objetivo**: acción observable, no implementación técnica.
**Beneficio**: valor de negocio explícito y medible.

### Ejemplo correcto
```text
Como comercial,
quiero confirmar un contrato y sincronizarlo automáticamente con SAP,
para reducir el tiempo de cierre de campaña y evitar recargas manuales.
```

### Ejemplo incorrecto
```text
Como usuario,
quiero que el sistema llame al método ActualizarContratoSAP,
para que funcione el botón.
```

---

## Criterios de aceptación

Usar Dado/Cuando/Entonces. Un escenario por criterio. Siempre cubrir:
- Camino feliz
- Validación / error esperado
- Caso borde relevante

```text
Dado que [contexto / precondición],
cuando [acción del usuario],
entonces [resultado verificable y observable].
```

### Ejemplo — confirmación de contrato
```text
Dado que un contrato está en estado "Borrador" con todos sus datos completos,
cuando el comercial confirma el contrato,
entonces el sistema lo pasa a estado "Confirmado" y dispara la actualización en SAP.

Dado que el contrato ya fue anulado,
cuando el comercial intenta confirmarlo,
entonces el sistema muestra un error indicando que un contrato anulado no puede confirmarse.

Dado que SAP no responde al momento de confirmar,
cuando el comercial confirma el contrato,
entonces el sistema registra el contrato como "Confirmado" localmente y encola la sincronización con SAP para reintento automático.
```

---

## Reglas de negocio

```text
RN-01: [restricción expresada como afirmación]
RN-02: [restricción]
```

### Ejemplo — contrato y CTG
```text
RN-01: Un contrato no puede finalizarse sin que todos sus boletos estén controlados.
RN-02: El CTG de un boleto debe confirmarse ante AFIP (WS GAQ) antes del cierre del recorrido.
RN-03: Un contrato anulado no puede volver a confirmarse.
RN-04: El peso/cantidad neta de un boleto no puede ser negativo ni superar lo declarado en la cláusula asociada.
RN-05: Un contrato no puede pasar a "PreAnulado" sin un motivo registrado.
```

> Nota: los ejemplos de este documento son ilustrativos del formato esperado, no reglas de negocio certificadas contra el código. Antes de dar una historia por cerrada, validar cada regla contra el Manager/entidad real involucrado.

---

## Análisis de gaps

Siempre revisar antes de cerrar los requerimientos:

### Checklist de gaps

- **Ambigüedades**: ¿hay términos con doble interpretación? (ej: "activo" — ¿activo en SAP o en el sistema?)
- **Escenarios faltantes**: ¿qué pasa si SAP no responde? ¿si el CTG no se confirma ante AFIP? ¿si se corta la conexión durante el proceso?
- **Conflictos entre reglas**: ¿alguna RN contradice otra? ¿hay prioridades entre reglas?
- **Condiciones de borde**: ¿qué pasa con cantidad/peso = 0? ¿con una campaña recién cerrada? ¿con materiales sin cupo disponible?
- **Preguntas abiertas**: listar explícitamente las preguntas que bloquean la implementación.
- **Integraciones**: ¿la historia depende de SAP, AFIP (CTG/WS GAQ) o Scato (logística de cupos)? ¿cómo se comporta si ese servicio falla?

### Template de gap

```text
❓ [Pregunta que bloquea la implementación]
   Contexto: [por qué es importante]
   Opciones: A) ... | B) ...
   Impacto si no se resuelve: [alto / medio / bajo]
```

---

## Priorización — criterios de negocio

Para recomendar prioridad, evaluar:

| Criterio | Peso |
|---|---|
| Impacto regulatorio (AFIP/SAP) | Crítico — bloquea la confirmación/cierre de contratos |
| Impacto en cierre de campaña o contratos | Alto — afecta el volumen operado en el período |
| Frecuencia de uso | Alto si es flujo principal (contrato, boleto, confirma) |
| Workaround disponible | Reduce urgencia si existe alternativa manual |
| Deuda técnica asociada | Aumenta urgencia si el cambio se encarece con el tiempo |
