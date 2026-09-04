---
name: patron-consulta-comando
description: 'Patrón Consulta/Comando de Entity Framework en Molinos.DataAgro.Repository/ConsultasEF (IConsulta<T>, IConsultaEscalar<T>, IComando<T>) y cómo cablearlo desde un Manager. Usar cuando pidan crear una nueva consulta EF, una clase Traer.../Devolver.../Buscar..., o optimizar una query moviéndola del Manager al Repository.'
---

# Patrón Consulta/Comando (Repository/ConsultasEF)

## Cuándo usar
- La query es compleja (joins, agregaciones, `TransactionScope`) y no entra cómoda en los métodos genéricos de `IRepositorio` (`Obtener`, `Listar`, etc.) con una expresión LINQ inline en el Manager.
- Se pide explícitamente crear/mover una clase de consulta a `Molinos.DataAgro.Repository/ConsultasEF`.

## Patrón

Elegir la interfaz según lo que devuelve la operación:
- `IConsulta<TEntidad>` → devuelve una lista.
- `IConsultaEscalar<TEntidad>` → devuelve un valor único (incluye el caso `DataSourceResult` de grids Kendo, ver skill [kendo-frontend](../kendo-frontend/SKILL.md)).
- `IComando<TResultado>` → ejecuta un side-effect (insert/update/delete con lógica propia).

```csharp
public class Traer{Algo} : IConsulta<{Entidad}>
{
    public Traer{Algo}(/* parámetros de filtro */) { ... }
    public virtual List<{Entidad}> Ejecutar(DbContext contexto) { ... }
}
```

## Procedimiento
1. Creá la clase en `Molinos.DataAgro.Repository/ConsultasEF`, con los parámetros de filtro como campos de solo lectura inyectados por constructor, y la query LINQ contra `DbContext` en el método `Ejecutar`.
2. Agregá o actualizá el método en `I{X}Manager` / `{X}Manager` (`Molinos.DataAgro.Business/Managers`) para invocar la consulta vía `repositorio.ListarConsulta(...)` (`IConsulta<T>`) u `repositorio.ObtenerConsultaEscalar(...)` (`IConsultaEscalar<T>`).
3. No agregues manejo de errores (try/catch) salvo que el Manager que estás tocando ya lo use.

## Fuera de alcance
- No inventes una convención de nombre de verbo (Traer/Devolver/Buscar/Obtener/Consultar): seguí el verbo que el usuario indique o el que ya predomine en consultas similares del mismo dominio.
- No modifiques `Molinos.DataAgro.DataContracts`, `Molinos.DataAgro.ServiceHost`, ni archivos de `WebDataAgro` (Controllers, Views) como parte de este flujo — eso es responsabilidad del Controller que consume el Manager, no de la Consulta/Comando.
- No agregues registro manual de DI: Autofac resuelve Managers automáticamente por convención de nombre.
