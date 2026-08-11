# Análisis de Campos de `Localidad` — Uso en Procesos SAP y Confirma

**Tabla origen**: `[dbo].[Localidad]`
**Entidad**: `Molinos.DataAgro.Entities\Entities\Localidad.cs`
**DTO**: `Molinos.DataAgro.Entities\Dto\LocalidadDto.cs`
**Servicio expuesto a SAP**: `WebDataAgro\Services\DataAgroServices.svc.cs` (interfaces `IDataAgroServices` / `IDataAgroServicesFull`)

Este documento responde, campo por campo, dónde y cómo se envían/reciben los valores de `Localidad` hacia/desde **SAP** (vía RFC y vía el servicio WCF `DataAgroServices`) y hacia **Confirma** (Web Service `LoteDocumentosService`).

---

## 1. Resumen ejecutivo

| Campo               | ¿SAP → DataAgro (entrada, vía RFC)?                                                                        | ¿DataAgro → SAP (salida)?                                | ¿DataAgro → Confirma?                                                        | ¿Expuesto por `DataAgroServices.svc` (`ListarLocalidades`)? |
|---------------------|:----------------------------------------------------------------------------------------------------------:|:--------------------------------------------------------:|:----------------------------------------------------------------------------:|:-----------------------------------------------------------:|
| **LocalidadId**     | ⚠️ Indirecto (se resuelve internamente a partir de `CodLocalidad`; SAP no lo envía)                        | ❌ No (ID interno de DataAgro, sin significado para SAP) | ❌ No (solo se usa para navegar a `Localidad`, no viaja como valor)         | ✅ Sí, se mapea (`LocalidadId`)                             |
| **CodLocalidad**    | ✅ Sí (clave de búsqueda, campo `Procedencia`)                                                             | ❌ No                                                    | ❌ No                                                                       | ❌ No (no se mapea en el DTO devuelto)                      |
| **Nombre**          | ❌ No                                                                                                      | ❌ No                                                    | ❌ No (se usa `CodigoPostal`/`SubCodigoPostal`, no el nombre)               | ✅ Sí, se mapea (`Nombre`)                                  |
| **ProvinciaId**     | ❌ No (SAP envía el código de provincia en `contratoSAP.Provincia`, no depende de `Localidad.ProvinciaId`) | ❌ No                                                    | ❌ No (Confirma usa `Provincia.CodigoConfirma`, no `Localidad.ProvinciaId`) | ✅ Sí, se mapea (`ProvinciaId`)                             |
| **PartidoId**       | ❌ No                                                                                                      | ❌ No                                                    | ❌ No                                                                       | ✅ Sí, se mapea (`PartidoId`)                               |
| **CodigoPostal**    | ❌ No                                                                                                      | ❌ No                                                    | ✅ Sí                                                                       | ❌ No (no se mapea en el DTO devuelto)                      |
| **SubCodigoPostal** | ❌ No                                                                                                      | ❌ No                                                    | ✅ Sí                                                                       | ❌ No (no se mapea en el DTO devuelto)                      |
| **CodigoConfirma**  | ❌ No                                                                                                      | ❌ No                                                    | ❌ No (existe en BD pero no se usa en el flujo de contrato actual)          | ❌ No (no se mapea en el DTO devuelto)                      |

> Nota: `Provincia_Nombre` y `Partido_Nombre` (también presentes en `LocalidadDto`) no son columnas de la tabla `Localidad`, sino datos de las tablas relacionadas `Provincia` y `Partido` que se completan junto con `ProvinciaId`/`PartidoId` en `ListarLocalidadTodas()`.

> ⚠️ **Hallazgo importante**: aunque `LocalidadDto` declara las propiedades `CodLocalidad`, `CodigoPostal`, `SubCodigoPostal` y `CodigoConfirma`, el método `ListarLocalidades()` del servicio WCF (consumido por SAP) **no las completa**. Ver sección 4.

---

## 2. `CodLocalidad` — Entrada desde SAP (RFC de alta/actualización de contrato)

**Dirección del dato**: SAP → DataAgro (no se envía a SAP, es una clave de matcheo).

Cuando SAP crea o actualiza un contrato y llama al servicio expuesto por `DataAgroServices.svc.cs`, envía el campo **`Procedencia`** dentro del objeto `contratoSAP`. DataAgro utiliza ese valor para localizar el registro de `Localidad` correspondiente mediante `CodLocalidad`:

```csharp
// WebDataAgro\Services\DataAgroServices.svc.cs (línea ~465)
contrato.LocalidadId = repositorio.Obtener<Localidad, int>(
    x => x.CodLocalidad == contratoSAP.Procedencia,
    x => x.LocalidadId);
```

- **RFC/Método expuesto**: método de alta/actualización de contrato en `DataAgroServices` (recibe el DTO `contratoSAP` con el campo `Procedencia`).
- **Uso**: `CodLocalidad` actúa como **código de matcheo único** (existe un índice único `IX_Localidad` sobre este campo) entre el maestro de localidades de SAP y el de DataAgro.
- **No se envía de vuelta a SAP** ni a Confirma directamente; solo se usa para resolver el `LocalidadId` interno.

---

## 3. `CodigoPostal` y `SubCodigoPostal` — Salida hacia Confirma

**Dirección del dato**: DataAgro → Confirma (Web Service `LoteDocumentosService`, método `AltaDefinitiva`).

Estos dos campos **no se envían a SAP**. Se utilizan exclusivamente para construir el campo compuesto `LocalidadConfirma` que se envía a Confirma como parte del XML del "Lote de Documentos".

### 3.1 Mapeo (Origen del dato)

```csharp
// Molinos.DataAgro.Repository\ConsultasEF\TraerTodosContratosSinFiltro.cs (línea 252)
// y TraerTodosContratosSinFiltroOptimizado.cs (equivalente)
LocalidadConfirma = contrato is FijacionDePrecioContrato
    ? (contrato as FijacionDePrecioContrato).Contrato.Localidad.CodigoPostal + (contrato as FijacionDePrecioContrato).Contrato.Localidad.SubCodigoPostal
    : contrato.Localidad.CodigoPostal + contrato.Localidad.SubCodigoPostal,
```

Este valor viaja en el DTO `BasicoContrato.LocalidadConfirma` (`Molinos.DataAgro.Entities\Dto\BasicoContrato.cs`, línea 282).

### 3.2 Envío al Web Service de Confirma

```csharp
// Molinos.DataAgro.Agent\Helpers\ConfirmaLoteDocumentosAgent.cs (línea ~654-661)
private Origen ConstruirOrigen(BasicoContrato contrato)
{
    return new Origen
    {
        LocalidadOrigen = new OrigenLocalidadOrigen { Value = contrato.LocalidadConfirma, LocalidadText = "" },
        ProvinciaOrigen = new TCodCaption { CodLista = contrato.ProvinciaConfirma }
    };
}
```

- **Proceso**: `ConfirmaLoteDocumentosAgent.ConfirmaLoteDocumentos()` → `ConstruirDocumentoXXX()` → `ConstruirOrigen()` → nodo `<Origen><LocalidadOrigen Value="{CodigoPostal}{SubCodigoPostal}"/></Origen>` del XML `Lote`.
- **Servicio destino**: `LoteDocumentosServiceClient.AltaDefinitiva(lote)` (WS SOAP externo de Confirma).
- **Campos combinados**: el valor enviado es la **concatenación simple** `CodigoPostal + SubCodigoPostal` (sin separador).

---

## 4. `CodigoConfirma` — Sin uso actual en el flujo de contratos

**Dirección del dato**: Ninguna en el flujo actual de contratos SAP/Confirma.

- El campo existe en la tabla `Localidad` y en la entidad/DTO correspondientes, pero **no aparece mapeado** en:
  - `TraerTodosContratosSinFiltro.cs` / `TraerTodosContratosSinFiltroOptimizado.cs` (no se copia a `BasicoContrato`).
  - `ConfirmaLoteDocumentosAgent.cs` (no se referencia en ningún `Construir...()`).
  - `DataAgroServices.svc.cs` (no se envía en ninguna respuesta a SAP).
- **Diferencia con `Centro.CodigoConfirma`**: existe un campo homónimo en la entidad `Centro` (`Molinos.DataAgro.Business\Managers\CentroManager.cs`) que **sí** se expone en `CentroDto` a través de `TraerCentro()` / `TraerTodoCentro()`, pero corresponde a una entidad distinta (Centro/Planta, no Localidad) y no debe confundirse.
- **Conclusión**: `Localidad.CodigoConfirma` es, a la fecha del análisis, un campo **inerte** en el flujo de integración: se persiste en base de datos pero ningún proceso lo lee para enviarlo a SAP o Confirma.

---

## 5. Servicio expuesto a SAP: `ListarLocalidades()`

**Interfaz**: `IDataAgroServices.ListarLocalidades()` / `IDataAgroServicesFull.ListarLocalidades()`
**Implementación**: `DataAgroServices.svc.cs` (línea ~2056)

```csharp
public List<LocalidadDto> ListarLocalidades()
{
    var resultado = localidadManager.ListarLocalidadTodas();
    return resultado;
}
```

Este es el **método RFC/WCF que SAP consume** para obtener el maestro de localidades. Sin embargo, el mapeo real (`LocalidadManager.ListarLocalidadTodas()`) solo completa estos campos:

```csharp
// Molinos.DataAgro.Business\Managers\LocalidadManager.cs (línea 167-178)
public List<LocalidadDto> ListarLocalidadTodas()
{
    return repositorio.Listar<Localidad, LocalidadDto>(x => new LocalidadDto
    {
        LocalidadId = x.LocalidadId,
        Nombre = x.Nombre,
        Provincia_Nombre = x.Provincia.Nombre,
        ProvinciaId = x.ProvinciaId,
        Partido_Nombre = x.Partido.Descripcion,
        PartidoId = x.PartidoId
        // NO se mapean: CodLocalidad, CodigoPostal, SubCodigoPostal, CodigoConfirma
    }, x => true);
}
```

### Consecuencia para la consulta de SAP

Cuando **SAP consulta el listado de localidades** a través de este método, **NO recibe** `CodLocalidad`, `CodigoPostal`, `SubCodigoPostal` ni `CodigoConfirma`, aun cuando el DTO tiene esas propiedades disponibles (quedan en `null`/vacío en la respuesta).

Si SAP necesita alguno de estos campos por este canal, **se requiere modificar** `LocalidadManager.ListarLocalidadTodas()` para incluir las propiedades faltantes en la proyección.

---

## 6. Diagrama de flujo

```
┌─────────────────────────────────────────────────────────────────-------┐
│                         SAP (ERP)                                      │
└───────────────-┬──────────────────────────-─────----┬──────────────────┘
                 │ RFC: Alta/Act. Contrato            │ RFC: ListarLocalidades()
                 │ (envía Procedencia = CodLocalidad) │     (consulta maestro)
                 ▼                                    ▼
   ┌─────────────────────────────-┐   ┌───────────────────────────----──┐
   │ DataAgroServices.svc.cs      │   │ DataAgroServices.svc.cs         │
   │  · Match CodLocalidad        │   │  ListarLocalidades()            │
   │    → LocalidadId             │   │  → LocalidadManager             │
   └───────────────┬──────────────┘   │     .ListarLocalidadTodas()     │
                   │                  │  (NO incluye CodLocalidad,      │
                   ▼                  │   CodigoPostal, SubCodigoPostal,│
     Contrato.LocalidadId asignado    │   CodigoConfirma)               │
                   │                  └─────────────────────────────----┘
                   ▼
   ┌─────────────────────────────────────────--┐
   │ BasicoContrato.LocalidadConfirma =        │
   │   Localidad.CodigoPostal + SubCodigoPostal│
   └───────────────────-┬──────────────────────┘
                        ▼
   ┌─────────────────────---────────────────────┐
   │ ConfirmaLoteDocumentosAgent.cs             │
   │  ConstruirOrigen() → LocalidadOrigen.Value │
   └──────────────────-─┬──────────────────────-┘
                        ▼
              ┌───────────────────┐
              │   Confirma (WS)   │
              │ AltaDefinitiva()  │
              └───────────────────┘
```

---

## 7. Respuesta directa para SAP

**Pregunta de SAP**
1) ¿Qué campo usan para matchear la localidad que envían por RFC?
`CodLocalidad` (campo `Procedencia` en el payload de contrato), comparado contra `Localidad.CodLocalidad`.

2) ¿Le devolvemos el Código Postal / SubCódigo Postal por RFC?
No. Solo se usan internamente para armar el dato que se envía a **Confirma** (`LocalidadConfirma = CodigoPostal + SubCodigoPostal`).

3) ¿Usamos `CodigoConfirma` en algún envío a SAP o Confirma?
No, actualmente no está integrado a ningún proceso de contrato.

4) ¿El método `ListarLocalidades()` (que SAP consume) devuelve estos 4 campos?
Solo devuelve `LocalidadId`, `Nombre`, `Provincia_Nombre`, `ProvinciaId`, `Partido_Nombre`, `PartidoId`. **No** incluye `CodLocalidad`, `CodigoPostal`, `SubCodigoPostal` ni `CodigoConfirma` (requiere cambio de código si SAP los necesita por este canal).

---

## 8. Archivos clave referenciados

- `Base de Datos\dbo\Tables\Localidad.sql`
- `Molinos.DataAgro.Entities\Entities\Localidad.cs`
- `Molinos.DataAgro.Entities\Dto\LocalidadDto.cs`
- `Molinos.DataAgro.Entities\Dto\BasicoContrato.cs`
- `Molinos.DataAgro.Business\Managers\LocalidadManager.cs`
- `Molinos.DataAgro.Repository\ConsultasEF\TraerTodosContratosSinFiltro.cs`
- `Molinos.DataAgro.Repository\ConsultasEF\TraerTodosContratosSinFiltroOptimizado.cs`
- `Molinos.DataAgro.Agent\Helpers\ConfirmaLoteDocumentosAgent.cs`
- `WebDataAgro\Services\DataAgroServices.svc.cs`
- `WebDataAgro\Services\IDataAgroServices.cs` / `IDataAgroServicesFull.cs`
