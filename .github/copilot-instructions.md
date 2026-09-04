# DataAgro — Guía para Copilot

## 1. Mandato principal

- Cuando corresponda, seguir la metodología SDD para features nuevas y continuaciones: ver `_sdd/docs/specs.md` y respetar los límites de [AGENTS.md](../AGENTS.md).
- Para arrancar o continuar una feature, usar el prompt `/sdd-workflow` cuando corresponda.
- Generar tests para toda lógica de negocio nueva o modificada.
- No instalar dependencias ni paquetes ni cambiar contratos públicos sin aprobación explícita cuando implique riesgo de integración.

## 2. Arquitectura (n-capas)

- `Molinos.DataAgro.DataContracts` / `Molinos.DataAgro.Entities`: DTOs y entidades compartidas.
- `Molinos.DataAgro.Interfaces/Managers`: un `I{X}Manager` por cada Manager de negocio.
- `Molinos.DataAgro.Business/Managers`: implementación de la lógica de negocio (`{X}Manager : I{X}Manager`).
- `Molinos.DataAgro.Repository`: acceso a datos con Entity Framework. Incluye `ConsultasEF/` con clases que implementan `IConsulta<T>`, `IComando<T>` o `IConsultaEscalar<T>` (método `Ejecutar(DbContext)`), usadas para queries complejas que no entran en los métodos genéricos de `IRepositorio`.
- `Molinos.DataAgro.ServiceHost`: fachada WCF (`IDataAgroServices`). No es el canal principal; expone solo operaciones puntuales para integración con SAP u otros sistemas.
- `WebDataAgro`: aplicación MVC, consumidor principal de los Managers vía Controllers.
- `Molinos.DataAgro.Test`: tests unitarios con NUnit + Moq; la convención es `{X}ManagerTest.cs` dentro de `Managers/`.

## 3. Patrones de diseño que se usan en el repo

### Convención de Managers

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

- El manejo de errores con `try/catch + logger.Error` no es un estándar obligatorio para todos los Managers.
- No asumir que cada Manager debe envolver su lógica en try/catch solo porque existe ese patrón en algunos casos.
- Si un Manager concreto ya usa manejo de errores, mantener ese estilo; si no, no inventar un patrón nuevo salvo que sea necesario para la funcionalidad que se está tocando.

### Inyección de dependencias (Autofac)

`WebDataAgro/App_Start/Startup.Dependencias.cs` registra automáticamente por convención de nombre:

- Toda clase que termina en `Manager` en `Molinos.DataAgro.Business` → registrada `AsImplementedInterfaces()`.
- Toda clase que termina en `Agent` en `Molinos.DataAgro.Agent` → registrada igual.

Si se crea un nuevo `{X}Manager : I{X}Manager` siguiendo esa convención, queda disponible automáticamente para inyectar en Controllers u otros Managers sin tocar `Startup.Dependencias.cs`.

### Patrón Consulta/Comando (Repository/ConsultasEF)

Para queries EF complejas (joins, agregaciones, `TransactionScope`), se crea una clase dedicada en `Molinos.DataAgro.Repository/ConsultasEF` en vez de escribir LINQ inline en el Manager, implementando `IConsulta<T>`, `IConsultaEscalar<T>` o `IComando<T>`. Ver skill [patron-consulta-comando](skills/patron-consulta-comando/SKILL.md) para el detalle completo del patrón y el procedimiento.
Se invoca desde el Manager con:

```csharp
repositorio.ListarConsulta(new Traer{Algo}(...));
```

Los nombres de estas clases no siguen una única convención rígida (`Traer`, `Devolver`, `Buscar`, `Obtener`, `Consultar`); no imponer una nueva convención sobre el código existente.

### Permisos en Controllers

Las acciones/controllers protegidos usan `[Autorizacion(PermisosDataAgro.X, ...)]`, que es el atributo propio sobre `AuthorizeAttribute` ubicado en `WebDataAgro/Atributos/AutorizacionAttribute.cs`.

- No usar directamente la propiedad `Roles` heredada.
- Para mostrar/ocultar UI o ramas de código sin bloquear la acción completa, usar `PermisosHelper.Is(PermisosDataAgro.X)` dentro de Controllers/Vistas.

## 4. Build, test y despliegue

### Build

```bash
# Build de la solución principal
msbuild DataAgro.sln /p:Configuration=Debug

# Build de release
msbuild DataAgro.sln /p:Configuration=Release

# Build de un proyecto específico
msbuild Molinos.DataAgro.Business/Molinos.DataAgro.Business.csproj
```

### Tests

Los tests usan NUnit y pueden ejecutarse desde Visual Studio o línea de comando:

```bash
# Ejecutar todos los tests
nunit3-console Molinos.DataAgro.Test\bin\Debug\Molinos.DataAgro.Test.dll

# Ejecutar tests por namespace o patrón
nunit3-console Molinos.DataAgro.Test\bin\Debug\Molinos.DataAgro.Test.dll --where "test.Namespace.Contains('HomeController')"

# Ejecutar un fixture concreto
nunit3-console Molinos.DataAgro.Test\bin\Debug\Molinos.DataAgro.Test.dll --where "test.Class == 'Molinos.DataAgro.Test.Controllers.HomeControllerTest'"
```

### Base de datos y deployment

- El proyecto de base se encuentra en `Base de Datos/`.
- Para despliegues, usar los scripts de `Molinos.DataAgro.Build/DeployBat/`.

```bash
DataAgro.web.bat
```

- Los parámetros por entorno están en archivos `.DeployParameters.xml` (DEV, QA, PROD, etc.).

## 5. Stack tecnológico

- .NET Framework 4.7.2.
- Entity Framework 6.
- Autofac 6.5.0.
- Hangfire 1.8.20.
- NLog 4.5.6.
- jQuery 1.10.2.
- Bootstrap 3.0.0.
- Kendo UI: JS estático en `WebDataAgro/Scripts/kendo/` (licencia comercial, sin NuGet).
- ActiveReports 6 (`DataDynamics.ActiveReports`).
- NUnit / Moq: 3.11.0 / 4.10.0.

Evitar sugerir APIs o sintaxis de versiones más nuevas que no existan en este proyecto.

## 6. UI, Kendo y notificaciones

- La UI principal suele usar MVC + Kendo UI + jQuery AJAX.
- Para grillas, combos, autocompletados, fechas y campos numéricos, respetar patrones existentes de Kendo UI y las clases/JS del proyecto.
- Para mensajes de usuario, preferir los patrones del repositorio con `BootstrapDialog` (ver [notificaciones-usuario-js.instructions.md](instructions/notificaciones-usuario-js.instructions.md)).

## 7. Rutas y artefactos clave

- Interfaz de Manager: `Molinos.DataAgro.Interfaces/Managers/I{X}Manager.cs`
- Implementación de Manager: `Molinos.DataAgro.Business/Managers/{X}Manager.cs`
- Consulta/Comando EF: `Molinos.DataAgro.Repository/ConsultasEF/{Verbo}{Algo}.cs`
- Integración externa: `Molinos.DataAgro.Agent/Helpers/{X}Agent.cs` + `I{X}Agent` en `Molinos.DataAgro.Interfaces/Agent`
- Job Hangfire: `WebDataAgro/Jobs/{Nombre}HangfireJob.cs` + registro del cron en `WebDataAgro/App_Start/RegisterHangfireJobs.cs`
- Controller/Vista/JS de una grilla: `WebDataAgro/Controllers/{X}Controller.cs`, `WebDataAgro/Views/{X}/*.cshtml`, `WebDataAgro/Scripts/App/{Feature}.js`
- Tabla + entidad EF: SQL en `Base de Datos/dbo/Tables/{Tabla}.sql` y entidad en `Molinos.DataAgro.Entities`
- Procesador de cláusula: `Molinos.DataAgro.Business/Clausulas` o `ClausulasBoleto`
- Reporte exportable: `Molinos.DataAgro.Report/Reportes/{Nombre}.cs` + wiring en el Manager
- Test de Manager: `Molinos.DataAgro.Test/Managers/{X}ManagerTest.cs`
- Documentación funcional: Wiki de Azure DevOps que corresponde a cada módulo

## 8. Inventario de customizations

### Skills

- `nueva-operacion-manager`: agregar un método nuevo en un Manager con su interfaz y Consulta EF si aplica.
- `kendo-frontend`: grillas, combo, autocompletado, filtros, fechas y campos numéricos con Kendo UI.
- `nuevo-job-hangfire`: job programado/background.
- `nuevo-procesador-clausula`: procesador de cláusulas contractuales.
- `nuevo-reporte-exportable`: lógica de datos de un reporte.
- `nuevo-agent-integracion-externa`: integración con SAP u otra API externa.
- `nueva-tabla-entidad-ef`: tabla + entidad EF nueva.
- `nuevo-modal-bootstrap`: modal/ventana emergente en una vista.
- `nueva-pantalla-menu`: pantalla nueva en el menú protegida por permiso.
- `documentar-funcionalidad-wiki`: documentación funcional en la Wiki con versionado.
- `user-story`: historia de usuario y criterios de aceptación.
- `release-notes`: notes de release para producción.

### Agents y prompts

- `consulta-builder`: clases de Consulta/Comando EF y su wiring en el Manager.
- `kendo-grid-builder`: grilla Kendo server-driven de punta a punta.
- `agent-integracion-builder`: integración externa nueva.
- `product-owner`: definición/refinamiento de historias y reglas de negocio.
- `architect`: revisión arquitectónica y refinamiento de diseño.
- `release-manager`: changelog y notas de release.

## 9. Glosario de dominio

- `Boleto`: documento de compra/venta de granos.
- `Contrato`: acuerdo comercial que agrupa boletos, con cláusulas asociadas.
- `Cláusula`: regla contractual procesada por el motor de cláusulas (`Clausulas` / `ClausulasBoleto`).
- `Campaña`: ciclo agrícola/comercial, por ejemplo campaña 23/24.
- `Material`: tipo de grano/producto agrícola.
- `Posición`: posición de entrega/mercado asociada a un contrato.
- `Contacto Comercial`: entidad de contacto para gestión comercial.
- `Agenda`: módulo de agenda/actividades comerciales.

## 10. Azure DevOps

Este proyecto usa Azure DevOps (organización `molinosagro`, proyecto `DataAgro`). Si hay herramientas MCP de Azure DevOps disponibles (wiki, work items, repos, PRs), verificar si existe algo relevante antes de recurrir a otra alternativa.

Para documentar un cambio o funcionalidad en la Wiki (`DataAgro.wiki`) con historial de versiones, seguir el skill `documentar-funcionalidad-wiki`.

## 11. Reglas de estilo y seguridad

- El código y los comentarios suelen estar en español.
- Mantener el estilo del código existente cuando se extiende funcionalidad.
- No desactivar validaciones de seguridad, CORS, autorización o JWT.
- No subir contraseñas, tokens, API keys, `.env` ni secretos.
- No marcar requisitos manuales como cubiertos sin sign-off humano.
- Antes de cambiar firma de controladores o contratos externos, validar impacto y pedir aprobación si aplica.

