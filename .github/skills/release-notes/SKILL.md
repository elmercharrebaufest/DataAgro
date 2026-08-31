---
name: release-notes
description: "Clasificación de cambios, formato de CHANGELOG y plantillas de release notes para DataAgro. Usar al generar una entrada de changelog o notas de release de un pase a producción."
---

# Release Notes — DataAgro

## Formato de versión

DataAgro no usa versionado semántico ni branches `release/*`. La "versión" de un release es el tag real que marca el pase a producción:

```
pase-produccion-YYYY-MM-DD
```

Preparado en una rama `feature/{autor}/pase-prod-YYYYMMDD` que se mergea a `master`. La cadencia es irregular (no semanal): no asumir un tag por semana.

---

## Clasificación de cambios

Para clasificar cada cambio, mirar el título/descripción del PR en Azure DevOps (convención `DAT-XXXX - Descripción`, ver `commit-message.prompt.md`):

| Categoría | Criterios | Emoji |
|---|---|---|
| `feature` | Nueva funcionalidad, nueva pantalla, nuevo método de negocio en un Manager | ✨ |
| `fix` | Corrección de bug, manejo incorrecto de estado, error en cálculo | 🐛 |
| `sap` | Cambios en la integración con SAP (`Molinos.DataAgro.Agent`, sincronización de contratos/boletos) | 🔗 |
| `afip` | Cambios en la integración AFIP/CTG (WS_GAQ, `EnviarMailSinCtg`, envío de cupos) | 🏛️ |
| `migration` | Cambio SQL en `Base de Datos/dbo/` (tabla, vista, SP o script) | 🗄️ |
| `performance` | Optimización de queries (ej. mover lógica a `ConsultasEF`), reducción de N+1 | ⚡ |
| `security` | Corrección de vulnerabilidades, cambios de permisos (`PermisosDataAgro`, `[Autorizacion]`) | 🔒 |
| `clausulas` | Cambios en el motor de cláusulas contractuales (`Clausulas`/`ClausulasBoleto`) | ⚖️ |
| `jobs` | Cambios en tareas programadas Hangfire (`WebDataAgro/Jobs`) | ⏱️ |
| `config` | Cambios en `Web.config`, `App.config`, parámetros de entorno | 🔧 |
| `chore` | Refactor interno sin impacto funcional, actualización de dependencias/tools | 🔨 |

### Criterios de impacto para PO

| Nivel | Descripción | Acción requerida |
|---|---|---|
| 🔴 Breaking | Cambia comportamiento existente, requiere acción del usuario | Documentar pasos de migración |
| 🟠 Notable | Nueva funcionalidad visible o corrección de bug crítico | Incluir en release notes PO |
| 🟡 Minor | Mejora pequeña o fix menor | Solo en CHANGELOG técnico |
| ⚪ Chore | Cambio interno sin impacto visible | Omitir del resumen PO |

---

## Reglas para armar el changelog

- **Alcance**: tomar solo los commits `Merged PR NNNN: DAT-XXXX ...` entre dos tags `pase-produccion-*` consecutivos. **Ignorar** los merges de sincronización `dev`↔`qa` (no representan cambios propios).
- **Redacción**: reescribir el título del PR en una frase breve y prolija en español (los títulos reales a veces son escuetos o con errores de tipeo), sin inventar alcance que no esté en el PR/diff.
- **Fuente de detalle del PR**: si hay tools de Azure DevOps MCP disponibles en el workspace, usarlas para traer el detalle real del PR y del work item (`mcp_azuredevops_m_repo_pull_request`, `mcp_azuredevops_m_wit_work_item`) en vez de inferir solo del `git log`. Si no están disponibles, usar `git log`/`git diff` como fallback.

---

## Formato CHANGELOG.md — Keep a Changelog adaptado

Vive en la raíz del repo (`CHANGELOG.md`, no existe todavía — crearlo en el primer uso).

```markdown
# Changelog

Todos los pases a producción notables de DataAgro se documentan en este archivo.
Formato basado en [Keep a Changelog](https://keepachangelog.com/es/1.0.0/).
Versiones: tag `pase-produccion-YYYY-MM-DD`.

---

## [pase-produccion-2026-09-01]

### ✨ Novedades
- Nueva grilla de seguimiento de cupos con filtro por campaña. (PR #6102)

### 🐛 Correcciones
- La edición de contratos no actualizaba la calidad del material al guardar. (PR #6066)

### 🔗 SAP
- Se corrige el reintento de sincronización de boletos cuando SAP responde con timeout. (PR #6089)

### 🏛️ AFIP
- Se agrega reenvío automático de mail cuando el cupo queda sin CTG. (PR #6070)

### 🗄️ Migraciones de base de datos
- `Base de Datos/dbo/Tables/Cupo.sql` — Agrega columna `FechaReenvioCTG` nullable.

### ⚙️ Componentes afectados
- ✅ WebDataAgro
- ✅ Molinos.DataAgro.Agent
- ⬜ Base de Datos *(sin cambios)*
- ⬜ Hangfire Jobs *(sin cambios)*
- ⬜ Molinos.DataAgro.Report *(sin cambios)*

---

## [pase-produccion-2026-08-25]
...
```

### Reglas del CHANGELOG
- **Siempre prepend** — el pase más reciente va al inicio, debajo del header.
- **Una sección por categoría** — omitir secciones sin entradas.
- **Cada PR = una línea** — frase corta en español, imperativo o sustantivo. Al final: `(PR #NNNN)`.
- **Scripts de `Base de Datos/dbo/`** — siempre describir qué cambia el objeto SQL y su impacto (columna nueva, dato inicial, etc.).
- **Fecha** implícita en el tag (`pase-produccion-YYYY-MM-DD`); no agregar una fecha separada salvo que difiera del tag.

---

## Plantilla de Release Notes para PO

Lenguaje de negocio — sin nombres técnicos de clase, namespace ni tabla.
Audiencia: Comercial, Corredor, Administrativo, Administrador del sistema (ver roles reales en `skills/user-story/SKILL.md`).

```markdown
# Release {tag pase-produccion-YYYY-MM-DD} — DataAgro

## Qué hay de nuevo

### Para Comercial / Corredores
- [descripción en lenguaje de usuario]

### Para Administrativos
- [si aplica]

### Para Administradores del sistema
- [cambios de configuración, nuevos permisos/roles]

## Correcciones importantes
- [bugs corregidos con impacto visible para el usuario]

## Cambios en integraciones (SAP / AFIP)
- [solo si aplica — describir como impacto operativo, no como cambio técnico]

## Pasos manuales requeridos
> ⚠️ Esta sección solo aparece si hay acciones requeridas post-deploy.
- [ ] Ejecutar script de datos en producción antes de habilitar el módulo.
- [ ] Comunicar a Comercial sobre el nuevo campo X.

## Componentes que se actualizan en este pase
- Portal principal (WebDataAgro)
- Integraciones externas (SAP / AFIP)
- Base de datos
```

---

## Glosario de términos PO-friendly

Usar estos términos en las release notes para el PO, nunca los nombres técnicos:

| Término técnico | Término PO-friendly |
|---|---|
| `Negocio` / `Contrato` | contrato comercial |
| `Boleto` | boleto (documento de compra/venta de granos) |
| `Cláusula` | cláusula contractual |
| `Campaña` | campaña agrícola |
| `Material` | tipo de grano/producto |
| `Posición` | posición de entrega/mercado |
| `Cupo` | cupo/turno de entrega |
| `CTG` / `WS_GAQ` | constancia de transporte de granos (trámite ante AFIP) |
| `SAP` | sistema de gestión corporativo (SAP) |
| `Corredor` | corredor comercial |
| `Contacto Comercial` | contacto comercial |
| `Manager` / `Repositorio` / `ConsultasEF` | base de datos / lógica del sistema (omitir en notas PO) |

---

## Comandos de referencia rápida

```powershell
# Tags de pase a producción recientes
git --no-pager tag --sort=-creatordate | Select-String "pase-produccion" | Select-Object -First 10

# PRs mergeados entre dos tags (excluye merges de sincronización dev/qa)
git --no-pager log "pase-produccion-2026-08-25..pase-produccion-2026-09-01" --oneline --grep="Merged PR"

# Archivos SQL cambiados entre tags
git --no-pager diff "pase-produccion-2026-08-25..pase-produccion-2026-09-01" --name-only -- "Base de Datos/dbo/"

# Fecha del tag
git --no-pager log -1 --format="%ai" "pase-produccion-2026-09-01"
```

Si hay tools de Azure DevOps MCP disponibles en el workspace, preferirlas sobre `az` CLI para traer detalle de PR/work item:
- `mcp_azuredevops_m_repo_pull_request` — detalle de un PR por id (título, descripción, work items linkeados).
- `mcp_azuredevops_m_wit_work_item` — detalle del ticket `DAT-XXXX` (tipo, título, criterios de aceptación).
- `mcp_azuredevops_m_repo_search_commits` — buscar commits/PRs por texto o rango.
