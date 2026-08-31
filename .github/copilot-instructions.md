# DataAgro — Guía para Copilot

## Arquitectura (n-capas)

- `Molinos.DataAgro.DataContracts` / `Molinos.DataAgro.Entities`: DTOs y entidades compartidas.
- `Molinos.DataAgro.Interfaces/Managers`: un `I{X}Manager` por cada Manager de negocio.
- `Molinos.DataAgro.Business/Managers`: implementación de la lógica de negocio (`{X}Manager : I{X}Manager`).
- `Molinos.DataAgro.Repository`: acceso a datos con Entity Framework. Incluye `ConsultasEF/` con clases que implementan `IConsulta<T>`, `IComando<T>` o `IConsultaEscalar<T>` (método `Ejecutar(DbContext)`), usadas para queries complejas que no entran en los métodos genéricos de `IRepositorio`.
- `Molinos.DataAgro.ServiceHost`: fachada WCF (`IDataAgroServices`). **No es el canal principal**: expone solo un puñado de operaciones puntuales para integración con SAP. La mayoría de las features no pasan por acá.
- `WebDataAgro`: aplicación MVC, consumidor principal de los Managers vía Controllers.
- `Molinos.DataAgro.Test`: tests unitarios, convención `{X}ManagerTest.cs` en `Managers/`.

## Convención de Managers

```csharp
public class {X}Manager : I{X}Manager
{
    private readonly ILogger logger;       // NLog.ILogger
    private readonly IRepositorio repositorio;

    public {X}Manager(ILogger logger, IRepositorio repositorio)
    {
        this.logger = logger;
        this.repositorio = repositorio;
    }
}
```

- El manejo de errores (try/catch + `logger.Error`) **no es un estándar obligatorio**: existe en algunos Managers (ej. `ContactoComercialAuditoriaManager`) pero no en otros (ej. `AgendaManager`). No asumir que todos los Managers deben envolver su lógica en try/catch salvo que el Manager que se esté tocando ya lo haga.

## Inyección de dependencias (Autofac) — no requiere registro manual

`WebDataAgro/App_Start/Startup.Dependencias.cs` registra automáticamente por convención de nombre:
- Toda clase que termina en `Manager` en `Molinos.DataAgro.Business` → registrada `AsImplementedInterfaces()`.
- Toda clase que termina en `Agent` en `Molinos.DataAgro.Agent` → ídem.

Si se crea un nuevo `{X}Manager : I{X}Manager` siguiendo esa convención de nombres, **queda disponible automáticamente** para inyectar en Controllers u otros Managers, sin tocar `Startup.Dependencias.cs`.

## Patrón Consulta/Comando (Repository/ConsultasEF)

Para queries EF complejas (joins, agregaciones, `TransactionScope`), se crea una clase dedicada en `Molinos.DataAgro.Repository/ConsultasEF` en vez de escribir LINQ inline en el Manager:

```csharp
public class Traer{Algo} : IConsulta<{Entidad}>
{
    public Traer{Algo}(/* parámetros de filtro */) { ... }
    public virtual List<{Entidad}> Ejecutar(DbContext contexto) { ... }
}
```

Se invoca desde el Manager con `repositorio.ListarConsulta(new Traer{Algo}(...))`. Los nombres de estas clases no siguen un único verbo fijo (conviven `Traer`, `Devolver`, `Buscar`, `Obtener`, `Consultar`); no forzar una convención nueva sobre el código existente.

## Permisos en Controllers (WebDataAgro)

Las acciones/controllers protegidos usan `[Autorizacion(PermisosDataAgro.X, ...)]` (atributo propio sobre `AuthorizeAttribute`, en `WebDataAgro/Atributos/AutorizacionAttribute.cs`), nunca la propiedad `Roles` heredada directamente. Para mostrar/ocultar UI o ramas de código según permiso (sin bloquear la acción completa), se usa `PermisosHelper.Is(PermisosDataAgro.X)` inline en Controllers/Views.

## Build y tests

- Soluciones: `DataAgro.sln` (aplicación principal) y `DataAgroWCF.sln` (ServiceHost). Build vía MSBuild/Visual Studio.
- Tests unitarios en `Molinos.DataAgro.Test` (NUnit + Moq), un archivo `{X}ManagerTest.cs` por Manager en `Managers/`. La cobertura es parcial; no todos los Managers tienen test. Ver [test-managers.instructions.md](instructions/test-managers.instructions.md) para el template de test.

## Azure DevOps

Este proyecto usa Azure DevOps (organización `molinosagro`, proyecto `DataAgro`). Si hay tools de Azure DevOps MCP disponibles (wiki, work items, repos, pull requests), verificar siempre si hay una relevante para el pedido del usuario antes de recurrir a otra alternativa. Para documentar una funcionalidad o cambio en la Wiki (`DataAgro.wiki`) con historial de versiones, ver el skill [documentar-funcionalidad-wiki](skills/documentar-funcionalidad-wiki/SKILL.md).

## Inventario de customizations

**Skills** (`.github/skills/<nombre>/SKILL.md`):

| Skill | Usar cuando... |
|---|---|
| [`nueva-operacion-manager`](skills/nueva-operacion-manager/SKILL.md) | Agregar un método de negocio nuevo en un Manager (con su interfaz, y Consulta EF si aplica) |
| [`kendo-frontend`](skills/kendo-frontend/SKILL.md) | Agregar/modificar una grilla, combo, buscador con autocompletado, fecha o campo numérico con Kendo UI |
| [`nuevo-job-hangfire`](skills/nuevo-job-hangfire/SKILL.md) | Crear una tarea programada/background job nueva |
| [`nuevo-procesador-clausula`](skills/nuevo-procesador-clausula/SKILL.md) | Agregar un procesador de cláusula contractual nuevo (boletos/contratos) |
| [`nuevo-reporte-exportable`](skills/nuevo-reporte-exportable/SKILL.md) | Agregar la lógica de datos de un reporte exportable (PDF/Excel) ya diseñado visualmente |
| [`nuevo-agent-integracion-externa`](skills/nuevo-agent-integracion-externa/SKILL.md) | Agregar una integración nueva con SAP u otra API externa |
| [`nueva-tabla-entidad-ef`](skills/nueva-tabla-entidad-ef/SKILL.md) | Crear una tabla nueva en la base y su entidad EF correspondiente |
| [`nuevo-modal-bootstrap`](skills/nuevo-modal-bootstrap/SKILL.md) | Agregar un modal/popup/ventana emergente (Bootstrap Modal) en una vista |
| [`nueva-pantalla-menu`](skills/nueva-pantalla-menu/SKILL.md) | Agregar el link de una pantalla nueva al menú de navegación (`_Layout.cshtml`), protegido por permiso |
| [`documentar-funcionalidad-wiki`](skills/documentar-funcionalidad-wiki/SKILL.md) | Documentar una funcionalidad/cambio en la Wiki de Azure DevOps, con versionado |
| [`user-story`](skills/user-story/SKILL.md) | Definir o refinar una historia de usuario, criterios de aceptación o reglas de negocio antes de implementar |
| [`release-notes`](skills/release-notes/SKILL.md) | Generar una entrada de changelog o notas de release de un pase a producción |

**Agents** (`.github/agents/*.agent.md`, subagentes con tools acotadas):

| Agent | Especialidad |
|---|---|
| `consulta-builder` | Crea clases de Consulta/Comando EF en `Repository/ConsultasEF` y las cablea en el Manager |
| `kendo-grid-builder` | Arma una grilla Kendo server-driven de punta a punta (vista + JS + Controller + Manager + Repository) |
| `agent-integracion-builder` | Crea una integración externa nueva (`{X}Agent`) para SAP u otra API |
| `product-owner` | Define/refina historias de usuario, criterios de aceptación y reglas de negocio antes de implementar |
| `architect` | Diseño técnico: revisión de deuda técnica, refinamiento de diseño y documentación de arquitectura |
| `release-manager` | Genera `CHANGELOG.md`, notas de release para el PO y audita cambios SQL/componentes de un pase a producción |

**Prompts** (`.github/prompts/*.prompt.md`):

| Prompt | Qué genera |
|---|---|
| `commit-message` | Mensaje de commit siguiendo la convención del equipo (`DAT-XXXX - Descripción`) |
| `pr-description` | Descripción de Pull Request para Azure DevOps a partir de los cambios de la rama |

## Rutas exactas por artefacto

| Artefacto | Ruta |
|---|---|
| Interfaz de Manager | `Molinos.DataAgro.Interfaces/Managers/I{X}Manager.cs` |
| Implementación de Manager | `Molinos.DataAgro.Business/Managers/{X}Manager.cs` |
| Consulta/Comando EF | `Molinos.DataAgro.Repository/ConsultasEF/{Verbo}{Algo}.cs` |
| Integración externa | `Molinos.DataAgro.Agent/Helpers/{X}Agent.cs` (+ `I{X}Agent` en `Molinos.DataAgro.Interfaces/Agent`) |
| Job Hangfire | `WebDataAgro/Jobs/{Nombre}HangfireJob.cs` (registro del cron en `WebDataAgro/App_Start/RegisterHangfireJobs.cs`) |
| Controller/Vista/JS de una grilla | `WebDataAgro/Controllers/{X}Controller.cs`, `WebDataAgro/Views/{X}/*.cshtml`, `WebDataAgro/Scripts/App/{Feature}.js` |
| Tabla + entidad EF | Objeto SQL en `Base de Datos/dbo/Tables/{Tabla}.sql`, entidad en `Molinos.DataAgro.Entities` |
| Procesador de cláusula | `Molinos.DataAgro.Business/Clausulas` o `ClausulasBoleto` |
| Reporte exportable | `Molinos.DataAgro.Report/Reportes/{Nombre}.cs` (diseño ActiveReports) + wiring de datos en el Manager |
| Test de Manager | `Molinos.DataAgro.Test/Managers/{X}ManagerTest.cs` |
| Página de documentación funcional | Wiki `DataAgro.wiki`, path `/DataAgro/{Nombre del módulo}` (Azure DevOps) |

## Stack tecnológico

.NET Framework **4.7.2**. Versiones relevadas de `packages.config` (evitar sugerir APIs/sintaxis de versiones más nuevas que no existen acá):

| Tecnología | Versión |
|---|---|
| Autofac | 6.5.0 |
| Hangfire | 1.8.20 |
| NLog | 4.5.6 |
| jQuery | 1.10.2 |
| Bootstrap | 3.0.0 (con BootstrapDialog para notificaciones, ver [notificaciones-usuario-js.instructions.md](instructions/notificaciones-usuario-js.instructions.md)) |
| Kendo UI | JS estático en `WebDataAgro/Scripts/kendo/` (licencia comercial, sin paquete NuGet) |
| ActiveReports | 6 (`DataDynamics.ActiveReports`) |
| NUnit / Moq | 3.11.0 / 4.10.0 |

## Glosario de dominio

Lista inicial de términos verificados en código/wiki (ampliable):

| Término | Significado |
|---|---|
| `Boleto` | Documento de compra/venta de granos |
| `Contrato` | Acuerdo comercial que agrupa boletos, con cláusulas asociadas |
| `Cláusula` | Regla contractual procesada por el motor de cláusulas (`Clausulas`/`ClausulasBoleto`) |
| `Campaña` | Ciclo agrícola/comercial (ej. campaña 23/24) |
| `Material` | Tipo de grano/producto agrícola |
| `Posición` | Posición de entrega/mercado asociada a un contrato |
| `Contacto Comercial` | Entidad de contacto para gestión comercial (ver `ContactoComercialAuditoriaManager`) |
| `Agenda` | Módulo de agenda/actividades comerciales (`AgendaManager`) |

