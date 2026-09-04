---
name: nueva-operacion-manager
description: 'Workflow para agregar una nueva operación de negocio de punta a punta en DataAgro: interfaz de Manager, implementación, consulta EF si aplica, y consumo desde un Controller de WebDataAgro. Usar cuando pidan agregar un nuevo método de negocio, una nueva acción/feature en un Manager existente o nuevo, o exponer una nueva consulta a la UI.'
---

# Nueva operación de negocio en un Manager

## Cuándo usar
- Se pide agregar un método nuevo a un Manager existente (ej. "agregar un método para traer X filtrado por Y").
- Se pide crear un Manager nuevo para una entidad/feature que todavía no tiene uno.

## Procedimiento

1. **Definir el método en la interfaz** `I{X}Manager` en `Molinos.DataAgro.Interfaces/Managers/I{X}Manager.cs`. Si el Manager no existe, crear la interfaz nueva ahí.

2. **Implementar el método en `Business/Managers/{X}Manager.cs`.**
   - Constructor con `ILogger logger, IRepositorio repositorio` (ver patrón en [copilot-instructions](../../copilot-instructions.md)).
   - Para queries simples, usar los métodos genéricos de `IRepositorio` (`Obtener`, `Listar`, etc.) directamente con una expresión LINQ.
   - Para queries complejas (joins, agregaciones, performance crítica), ir al paso 3.
   - No agregar try/catch salvo que el Manager ya use ese patrón.

3. **(Si la query es compleja) Crear una clase en `Molinos.DataAgro.Repository/ConsultasEF`** siguiendo la skill [patron-consulta-comando](../patron-consulta-comando/SKILL.md) (`IConsulta<T>`/`IConsultaEscalar<T>`/`IComando<T>`).

4. **Consumir el método desde un Controller de `WebDataAgro`.**
   - Inyectar `I{X}Manager` por constructor (Autofac lo resuelve automáticamente por convención de nombre, no requiere tocar `Startup.Dependencias.cs`).
   - Si el Manager es nuevo, verificar que la clase se llame `{X}Manager` para que la registración automática lo detecte.

5. **(Opcional) Agregar/actualizar el test** en `Molinos.DataAgro.Test/Managers/{X}ManagerTest.cs`.

## Fuera de alcance
- **No exponer la operación vía WCF** (`Molinos.DataAgro.ServiceHost` / `IDataAgroServices`). Ese canal está reservado a un puñado de integraciones puntuales con SAP y no es parte de este flujo. Si el pedido es explícitamente sobre integración SAP, tratarlo como un caso aparte y confirmarlo con el usuario antes de tocar `ServiceHost`.
