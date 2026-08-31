# Copilot en DataAgro — Primeros pasos

## Objetivo

Esta es la primera vez que el equipo usa las customizaciones de GitHub Copilot en VS Code (**instructions**, **skills**, **agents** y **prompts**). Esta guía explica qué es cada una, para qué sirve, cómo se usa desde el chat, y qué tenemos hoy ya armado para DataAgro.

Todos estos archivos viven en la carpeta `.github/` en la raíz del repo y quedan versionados en git: al bajar la rama, cualquiera del equipo los tiene disponibles automáticamente en su Copilot Chat, sin instalar nada.

## Resumen rápido

| Herramienta | ¿Qué es? | ¿Cuándo actúa? | ¿Cómo se invoca? |
|---|---|---|---|
| **Instructions** | Reglas/convenciones que Copilot debe seguir | Siempre (si es global) o solo al tocar ciertos archivos | Automático, no se invoca a mano |
| **Skills** | Guía paso a paso para una tarea repetible del proyecto | Cuando Copilot detecta que la tarea aplica, o a pedido | Automático, o escribiendo `/nombre-skill` |
| **Agents** | Un "especialista" con un rol acotado y herramientas limitadas | Cuando se lo invoca explícitamente o el agente principal delega en él | Delegación automática, o mencionándolo |
| **Prompts** | Una acción puntual y parametrizada (genera un texto/resultado concreto) | Solo cuando se lo pide | Escribiendo `/nombre-prompt` |

## 1. Instructions (`*.instructions.md`, `copilot-instructions.md`)

Son **reglas de contexto** que Copilot lee automáticamente, sin que nadie las invoque. Hay dos variantes:

- **Siempre activas**: [`.github/copilot-instructions.md`](../../.github/copilot-instructions.md) — arquitectura general de DataAgro (capas, convención de Managers, DI por Autofac, patrón Consulta/Comando, build/tests, y qué hacer cuando hay tools de Azure DevOps MCP disponibles). Se aplica en **toda** conversación de Copilot en este repo.
- **Por archivo** (`applyTo` o `description`): se activan solo cuando corresponde.
  - [`test-managers.instructions.md`](../../.github/instructions/test-managers.instructions.md): se activa automáticamente al editar cualquier archivo bajo `Molinos.DataAgro.Test/**/*.cs` (convención NUnit + Moq).
  - [`subida-archivos.instructions.md`](../../.github/instructions/subida-archivos.instructions.md): se activa cuando la tarea es sobre subir/validar archivos (Excel, KMZ), sin importar en qué archivo se esté parado.
  - [`notificaciones-usuario-js.instructions.md`](../../.github/instructions/notificaciones-usuario-js.instructions.md): se activa cuando la tarea es mostrar un mensaje/alerta/confirmación al usuario desde JS (funciones de `Mastersoft.js` sobre BootstrapDialog).

**No hace falta hacer nada para usarlas**: si estás trabajando en un `*ManagerTest.cs`, Copilot ya sabe el patrón NUnit+Moq del proyecto sin que se lo expliques.

## 2. Skills (`SKILL.md`)

Son **guías de un flujo de trabajo repetible** de DataAgro, con los pasos concretos y las convenciones reales del código (qué carpeta, qué patrón, qué NO hacer). Copilot puede activarlas solo (si detecta que el pedido matchea) o se pueden invocar a mano escribiendo `/` en el chat y eligiendo la skill.

Skills disponibles hoy:

| Skill | Usar cuando... |
|---|---|
| [`nueva-operacion-manager`](../../.github/skills/nueva-operacion-manager/SKILL.md) | Agregar un método de negocio nuevo en un Manager (con su interfaz, y Consulta EF si aplica) |
| [`kendo-frontend`](../../.github/skills/kendo-frontend/SKILL.md) | Agregar/modificar una grilla, combo, buscador con autocompletado, fecha o campo numérico con Kendo UI |
| [`nuevo-job-hangfire`](../../.github/skills/nuevo-job-hangfire/SKILL.md) | Crear una tarea programada/background job nueva |
| [`nuevo-procesador-clausula`](../../.github/skills/nuevo-procesador-clausula/SKILL.md) | Agregar un procesador de cláusula contractual nuevo (boletos/contratos) |
| [`nuevo-reporte-exportable`](../../.github/skills/nuevo-reporte-exportable/SKILL.md) | Agregar la lógica de datos de un reporte exportable (PDF/Excel) ya diseñado visualmente |
| [`nuevo-agent-integracion-externa`](../../.github/skills/nuevo-agent-integracion-externa/SKILL.md) | Agregar una integración nueva con SAP u otra API externa |
| [`nueva-tabla-entidad-ef`](../../.github/skills/nueva-tabla-entidad-ef/SKILL.md) | Crear una tabla nueva en la base y su entidad EF correspondiente |
| [`nuevo-modal-bootstrap`](../../.github/skills/nuevo-modal-bootstrap/SKILL.md) | Agregar un modal/popup/ventana emergente (Bootstrap Modal) en una vista |
| [`nueva-pantalla-menu`](../../.github/skills/nueva-pantalla-menu/SKILL.md) | Agregar el link de una pantalla nueva al menú de navegación (`_Layout.cshtml`), protegido por permiso |
| [`documentar-funcionalidad-wiki`](../../.github/skills/documentar-funcionalidad-wiki/SKILL.md) | Documentar una funcionalidad/cambio en la Wiki de Azure DevOps (`DataAgro.wiki`), con versionado e historial dentro del mismo documento (requiere el MCP server de Azure DevOps, ver sección 5) |
| [`user-story`](../../.github/skills/user-story/SKILL.md) | Definir o refinar una historia de usuario, criterios de aceptación o reglas de negocio antes de implementar |
| [`release-notes`](../../.github/skills/release-notes/SKILL.md) | Generar una entrada de changelog o notas de release de un pase a producción |

**Ejemplo de uso**: en el chat, escribir `/nueva-operacion-manager` (o simplemente pedir en lenguaje natural "agregame un método para traer X filtrado por Y" — Copilot va a detectar sola que aplica esta skill).

## 3. Agents (`*.agent.md`)

Son **subagentes especializados**: tienen un rol acotado y solo pueden usar ciertas herramientas (por ejemplo, no pueden tocar archivos fuera de una carpeta puntual). Se usan para tareas mecánicas que cruzan varios archivos, manteniendo la conversación principal más limpia.

Agents disponibles hoy:

| Agent | Especialidad |
|---|---|
| `consulta-builder` | Crea clases de Consulta/Comando EF en `Repository/ConsultasEF` y las cablea en el Manager |
| `kendo-grid-builder` | Arma una grilla Kendo server-driven de punta a punta (vista + JS + Controller + Manager + Repository) |
| `agent-integracion-builder` | Crea una integración externa nueva (`{X}Agent`) para SAP u otra API |
| `product-owner` | Define/refina historias de usuario, criterios de aceptación y reglas de negocio, antes de implementar |
| `architect` | Diseño técnico: revisión de deuda técnica, refinamiento de diseño y documentación de arquitectura |
| `release-manager` | Genera `CHANGELOG.md`, redacta notas de release para el PO y audita cambios SQL/componentes de un pase a producción |

**Cómo se invocan**: se puede pedir explícitamente ("usá el agent consulta-builder para...") o el agente principal de Copilot puede delegar solo si detecta que el pedido coincide con la especialidad de alguno. Algunos agents tienen **handoffs** configurados: por ejemplo, `product-owner` puede pasarle el trabajo a `architect` una vez aprobados los requerimientos, y `release-manager` puede pedirle a `product-owner` que revise el lenguaje de las notas de release.

## 4. Prompts (`*.prompt.md`)

Son **acciones puntuales y parametrizadas**: a diferencia de una skill (que es una guía de varios pasos), un prompt genera un resultado concreto de una sola vez.

Prompts disponibles hoy:

| Prompt | Qué genera |
|---|---|
| [`commit-message`](../../.github/prompts/commit-message.prompt.md) | El mensaje de commit siguiendo la convención del equipo (`DAT-XXXX - Descripción`), a partir del diff actual |
| [`pr-description`](../../.github/prompts/pr-description.prompt.md) | Una descripción de Pull Request para pegar en Azure DevOps, a partir de los cambios de la rama |

**Cómo se invocan**: escribiendo `/commit-message` o `/pr-description` en el chat (o desde la paleta de comandos: `Chat: Run Prompt...`).

## 5. MCP Servers (`.vscode/mcp.json`)

Un **MCP server** le da a Copilot acceso a un sistema externo real (no es un archivo de convenciones como los anteriores, sino una integración con una API). Hoy tenemos configurado:

- **`ado-remote-mcp`**: servidor remoto oficial de Azure DevOps, apunta a la organización `molinosagro`. Da acceso a Wikis, Work Items, Repos y Pipelines directamente desde el chat (por ejemplo, listar/leer/crear páginas de la wiki `DataAgro.wiki`, usado por la skill `documentar-funcionalidad-wiki`).

**Cómo se activa**: paleta de comandos → `MCP: List Servers` → `ado-remote-mcp` → `Start Server` (la primera vez pide iniciar sesión con la cuenta que tiene acceso a `molinosagro`). Una vez iniciado, hay que habilitar sus tools en el selector de herramientas del chat.

## ¿Cómo pido que se cree un skill/agent/instruction nuevo?

Simplemente pedirlo en el chat en lenguaje natural (ej. "quiero un skill para agregar un nuevo tipo de notificación push"). Copilot va a:
1. Analizar el código real del proyecto para basar el flujo en patrones existentes (no inventar convenciones).
2. Mostrar un análisis funcional/técnico y las ambigüedades encontradas, con un plan guardado en un `.md`.
3. Crear los archivos recién después de que el equipo confirme el plan.

## Dónde están los archivos

```
.github/
├── copilot-instructions.md          ← siempre activo
├── instructions/*.instructions.md   ← activo por archivo o por descripción
├── skills/<nombre>/SKILL.md         ← flujo paso a paso, invocable con /
├── agents/*.agent.md                ← subagentes especializados
└── prompts/*.prompt.md              ← acciones puntuales, invocables con /

.vscode/
└── mcp.json                         ← servidores MCP (ej. ado-remote-mcp para Azure DevOps)
```
