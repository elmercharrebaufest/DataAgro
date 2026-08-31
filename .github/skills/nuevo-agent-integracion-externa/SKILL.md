---
name: nuevo-agent-integracion-externa
description: 'Workflow para crear una nueva integración externa (SAP u otra API) siguiendo el patrón {X}Agent de Molinos.DataAgro.Agent. Usar cuando pidan agregar una integración con SAP, una llamada a un webservice externo, o un nuevo Agent.'
---

# Nueva integración externa (Agent)

## Patrón

Cada integración externa es una clase `{X}Agent : I{X}Agent`:
- Interfaz en `Molinos.DataAgro.Interfaces/Agent/I{X}Agent.cs`.
- Implementación en `Molinos.DataAgro.Agent/Helpers/{X}Agent.cs`.
- Inyecta `ILogger` (NLog) por constructor.
- Lee credenciales/config de `ConfigurationManager.AppSettings["..."]` (ej. `SapUser`, `SapPass`).
- Instancia el cliente generado en `Service References/` (WCF/SOAP) o un `HttpClient` para APIs REST, invoca la operación y mapea el resultado a un DTO propio.

```csharp
public class {X}Agent : I{X}Agent
{
    private readonly ILogger logger;

    public {X}Agent(ILogger logger)
    {
        this.logger = logger;
    }

    public {TipoResultado} {Operacion}({Parametros})
    {
        var userSap = ConfigurationManager.AppSettings["SapUser"];
        var passSap = ConfigurationManager.AppSettings["SapPass"];

        var cliente = new {ClienteGeneradoWCF}();
        cliente.ClientCredentials.UserName.UserName = userSap;
        cliente.ClientCredentials.UserName.Password = passSap;

        var respuesta = cliente.{OperacionSap}(new {RequestSap} { /* ... */ });
        return {mapeo a DTO propio};
    }
}
```

## Procedimiento
1. Definir la interfaz `I{X}Agent` en `Molinos.DataAgro.Interfaces/Agent`.
2. Implementar `{X}Agent` en `Molinos.DataAgro.Agent/Helpers`, siguiendo el patrón de arriba.
3. Consumir el Agent inyectándolo en el Manager que lo necesite (DI automática por Autofac, sufijo `Agent`, sin registro manual).
4. Si la integración expone un servicio WCF (`Service References/`), agregarlo primero con "Add Service Reference" en Visual Studio antes de escribir el Agent (no es algo editable a mano).

## Constraints
- **No agregar credenciales nuevas en `Web.config`** sin preguntar antes al usuario dónde deben ir. El manejo de secretos del proyecto se está revisando aparte; no asumir que agregar una entrada más en `appSettings` en texto plano es aceptable para código nuevo.
- No inventar el contrato del servicio externo (nombres de operación, request/response): confirmarlos con el usuario o basarse en un Agent existente del mismo sistema externo.
