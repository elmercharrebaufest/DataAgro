# Recomendaciones para Método Manual desde Excel

## Resumen Comparativo

### Método `ObtenerNegocios` (Automático)
**Entrada**: Consulta BD para traer contratos con ciertos criterios  
**Flujo**: Filtrado → Cálculo → Validación → Limitación  
**Salida**: Sugerencias ordenadas por prioridad

### Nuevo Método (Manual desde Excel)
**Entrada**: Usuario proporciona lista con ContratoSAP, FechaSugerida, CantidadDeCupos  
**Flujo**: Lookup de Contrato → Cálculo KgPendiente → Validación de límites  
**Salida**: Sugerencias validadas o rechazadas con motivos

---

## Validaciones Necesarias

### ✅ OBLIGATORIAS (El método `ObtenerNegocios` las hace)

1. **Calcular KgPendienteAplicar**
   ```csharp
   List<ContratoKgPendiente> kgPendientes = contratoKgPendienteAgent.Consultar(
       new List<ContratoKgPendiente> 
       { 
           new ContratoKgPendiente 
           { 
               ContratoSAP = "123456789",
               ContratoId = contrato.Id 
           }
       }
   );
   sugerencia.KgPendienteAplicar = kgPendientes[0].KgPendiente;
   ```

   **Por qué**: Es el dato real de SAP que determina cuántos kilos están disponibles.

2. **Validar contra ConfiguracionCupo**
   ```csharp
   var config = repositorio.Obtener<ConfiguracionCupo>(
       x => x.CentroId == sugerencia.CentroId && 
            x.MaterialId == sugerencia.MaterialId &&
            x.Fecha == sugerencia.FechaSugerida.Date
   );

   if (totalSugerenciasDelDia > config.LimiteAlgoritmo)
       return Error("Supera límite configurado");
   ```

   **Por qué**: El `LimiteCupo` y `LimiteAlgoritmo` definen máximos por planta/día/material.

3. **Validar Cupos Pendientes del mismo Negocio**
   ```csharp
   var cuposPendientes = repositorio.Listar<Cupo>(
       x => x.NegocioId == contrato.Id &&
            x.Cumplimiento != true &&
            x.EstadoCupoId != (int)EnumEstadoCupo.Anulado &&
            x.EstadoCupoId != (int)EnumEstadoCupo.Rechazado &&
            x.FechaIngreso >= DateTime.Now.Date.AddDays(-1)
   ).Count();

   sugerencia.CantidadDeCupos -= cuposPendientes;
   ```

   **Por qué**: No puede sugerir cupos si ya hay cupos pendientes del mismo negocio.

4. **Validar CCPP Pendientes (CartasDePortePendiente)**
   ```csharp
   var ccpp = cartasDePortePendienteAplicarAgent.ListarCartasDePortePendienteAplicar(
       new CcPpPendienteAplicarDto
       {
           Centro = centro.CodigoSap,
           Material = material.Codigo,
           Proveedor = sugerencia.ProveedorCUIT,
           Corredor = sugerencia.CUITCorredor,
           AgenteCompra = ""
       }
   );

   var cantidadCCPP = ccpp
       .Where(x => !string.IsNullOrEmpty(x.CartasPorte) && string.IsNullOrEmpty(x.Contrato))
       .Sum(x => x.Cantidad);

   if (cantidadCCPP >= kilosMinimos)
   {
       cuposADescontar = (int)Math.Floor(cantidadCCPP / 30000);
       sugerencia.CantidadDeCupos -= cuposADescontar;
   }
   ```

   **Por qué**: Las CCPP pendientes representan mercadería que podría aplicarse, por lo que reservan cupos.

5. **Validar Stock Sustentable/EPA/EUDR**
   ```csharp
   if (contrato.Sustentable || contrato.EPA || contrato.EUDR)
   {
       int stockDisponible = TraerCuposDisponiblesEnEstablecimientos(
           sugerencia.ProveedorCUIT, 
           esEPAoEUDR: (contrato.EPA || contrato.EUDR)
       );

       // Descontar cupos pendientes del mismo tipo
       stockDisponible -= ObtenerCuposPendientesDelTipo(sugerencia.ProveedorCUIT, contrato.Sustentable);

       if (sugerencia.CantidadDeCupos > stockDisponible)
       {
           sugerencia.CantidadDeCupos = stockDisponible;
           sugerencia.Inhabilitado = "Stock insuficiente: " + stockDisponible + " cupos disponibles";
       }
   }
   ```

   **Por qué**: No pueden sugerir más cupos que el stock físico disponible.

---

### ❌ OPCIONALES (Depende de negocio)

- **Validar si es Fason**: El método automático los excluye. ¿Aplica aquí?
- **Validar Warranty/CD**: El método automático lo consulta pero no lo usa como filtro.
- **Validar Zona**: El método automático la obtiene, pero ¿necesitas validarla?

---

## Flujo Sugerido para Nuevo Método

```csharp
public Resultado ValidarSugerenciasDesdeExcel(
    List<SugerenciaCupoDto> sugerenciasExcel,
    int centroId,
    int materialId,
    DateTime fechaSugerida)
{
    var resultado = new Resultado();
    var sugerenciasValidas = new List<SugerenciaCupoDto>();

    // VALIDACIÓN 1: Configuración General
    var config = ObtenerConfiguracionCupo(centroId, materialId, fechaSugerida);
    var totalSugerencias = sugerenciasExcel.Sum(x => x.CantidadDeCupos);

    if (totalSugerencias > config.LimiteAlgoritmo)
    {
        resultado.Error("Límite General", 
            $"Total {totalSugerencias} supera límite {config.LimiteAlgoritmo}");
        return resultado;
    }

    // VALIDACIÓN 2: Por cada sugerencia
    foreach (var sugerencia in sugerenciasExcel)
    {
        var erroresItem = new List<string>();

        // 2.1: Validar que ContratoSAP exista
        var contrato = repositorio.Obtener<Negocio>(
            x => x.ContratoSAP == sugerencia.ContratoSAP &&
                 x.DestinoId == centroId &&
                 x.MaterialId == materialId
        );

        if (contrato == null)
        {
            erroresItem.Add($"Contrato {sugerencia.ContratoSAP} no encontrado");
            continue;
        }

        // 2.2: Obtener KgPendienteAplicar
        var kgPendiente = contratoKgPendienteAgent.Consultar(
            new List<ContratoKgPendiente> 
            { 
                new ContratoKgPendiente 
                { 
                    ContratoSAP = sugerencia.ContratoSAP,
                    ContratoId = contrato.Id 
                }
            }
        )[0].KgPendiente;

        sugerencia.KgPendienteAplicar = kgPendiente;

        // Validar mínimo de kilos
        int kilosMinimos = repositorio.Obtener<Configuracion>(1)
            .AlgoritmoKilosMinimosParaSugerencia * 1000;

        if (kgPendiente < kilosMinimos)
        {
            erroresItem.Add(
                $"Kilos pendientes ({kgPendiente}) menores a mínimo ({kilosMinimos})");
        }

        // 2.3: Validar Cupos Pendientes
        var cuposPendientes = repositorio.Listar<Cupo>(
            x => x.NegocioId == contrato.Id &&
                 x.Cumplimiento != true &&
                 x.EstadoCupoId != (int)EnumEstadoCupo.Anulado &&
                 x.EstadoCupoId != (int)EnumEstadoCupo.Rechazado &&
                 x.FechaIngreso >= DateTime.Now.Date.AddDays(-1)
        ).Count();

        sugerencia.CuposPendientes = cuposPendientes;
        if (cuposPendientes > 0)
        {
            sugerencia.CantidadDeCupos = Math.Max(0, 
                sugerencia.CantidadDeCupos - cuposPendientes);
            erroresItem.Add($"Se descontaron {cuposPendientes} cupos pendientes");
        }

        // 2.4: Validar CCPP Pendientes
        var ccpp = ValidarCCPPPendientes(contrato, sugerencia);
        if (ccpp.cuposADescontar > 0)
        {
            sugerencia.CantidadDeCupos = Math.Max(0, 
                sugerencia.CantidadDeCupos - ccpp.cuposADescontar);
            erroresItem.Add($"Se descontaron {ccpp.cuposADescontar} " +
                "cupos por CCPP pendientes");
        }

        // 2.5: Validar Stock si es Sustentable/EPA/EUDR
        if (contrato.Sustentable || contrato.EPA || contrato.EUDR)
        {
            var stockValida = ValidarStockEspecial(contrato, sugerencia);
            if (!stockValida.valido)
            {
                sugerencia.CantidadDeCupos = stockValida.cantidadMaxima;
                erroresItem.Add(stockValida.motivo);
            }
        }

        // Registrar validaciones
        if (sugerencia.CantidadDeCupos <= 0)
        {
            resultado.Error($"Contrato {sugerencia.ContratoSAP}", 
                string.Join(" | ", erroresItem));
        }
        else
        {
            sugerencia.Inhabilitado = string.Join(" | ", erroresItem);
            sugerenciasValidas.Add(sugerencia);
        }
    }

    if (sugerenciasValidas.Count > 0)
    {
        // Guardar sugerencias validadas
        repositorio.AgregarTodos(
            sugerenciasValidas.Select(x => new SugerenciaCupo
            {
                CentroId = centroId,
                MaterialId = materialId,
                FechaSugerida = fechaSugerida,
                CantidadDeCupos = x.CantidadDeCupos,
                CantidadCupoOriginal = x.CantidadDeCupos,
                NegocioId = x.NegocioId,
                TipoNegocioId = x.TipoNegocioId,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Puntuacion = 0,
                ProveedorId = x.ProveedorId,
                StandardDeCalidad = x.StandardDeCalidad,
                ZonaCupoId = x.ZonaCupoId,
                Destinatario = x.Destinatario,
                ComercialId = x.ComercialId,
                ContratoSAP = x.ContratoSAP,
                ConfiguracionEspacioDinamicoId = x.ConfiguracionEspacioDinamicoId,
                KgNegocio = x.KgNegocio,
                KgPendienteAplicar = x.KgPendienteAplicar,
                Aceptado = null,
                CDWarrant = x.CDWarrant
            }).ToList()
        );
        repositorio.GuardarCambios();
    }

    return resultado;
}
```

---

## Campos que DEBES Completar

Cuando crees las `SugerenciaCupo` desde Excel, asegúrate de llenar:

| Campo                | Fuente                                | Obligatorio          |
|----------------------|---------------------------------------|----------------------|
| `NegocioId`          | Lookup por ContratoSAP                | ✅                   |
| `ContratoSAP`        | Del Excel                             | ✅                   |
| `CantidadDeCupos`    | Del Excel (validado)                  | ✅                   |
| `FechaSugerida`      | Del Excel                             | ✅                   |
| `KgPendienteAplicar` | ContratoKgPendienteAgent              | ✅                   |
| `KgNegocio`          | Negocio.Cantidad                      | ✅                   |
| `MaterialId`         | Parámetro / Negocio                   | ✅                   |
| `CentroId`           | Parámetro / Negocio                   | ✅                   |
| `ProveedorId`        | Negocio.ProveedorId (o Corredor)      | ✅                   |
| `ComercialId`        | Negocio.ComercialId                   | ✅                   |
| `TipoNegocioId`      | Negocio.TipoNegocioId                 | ✅                   |
| `ZonaCupoId`         | Buscar zona por descripción           | ✅                   |
| `Destinatario`       | "30715118773" (constante)             | ✅                   |
| `MonedaId`           | Negocio.MonedaId                      | ⚠️                   |
| `Precio`             | Negocio.Precio                        | ⚠️                   |
| `StandardDeCalidad`  | Negocio.StandardDeCalidad.Descripción | ⚠️                   |
| `Sustentable`        | Negocio.Sustentable                   | ✅ (para validación) |
| `EPA`, `EUDR`        | Negocio.EPA/EUDR                      | ✅ (para validación) |
| `CDWarrant`          | ❌ (opcional)                         | ⚠️                   |
| `Aceptado`           | null (pendiente usuario)              | ✅                   |

---

## Recomendación Final

**Crea dos métodos**:

1. **`ValidarSugerenciasDesdeExcel()`**: 
   - Valida que todas las sugerencias cumplan restricciones
   - Devuelve `Resultado` con errores detallados
   - NO guarda nada

2. **`GuardarSugerenciasDesdeExcel()`**: 
   - Llama a (1) para validar
   - Si no hay errores, crea registros `SugerenciaCupo`
   - Si hay errores, devuelve lista de problemas sin guardar

Esto permite al usuario corregir el Excel y reintentar sin dañar datos.

