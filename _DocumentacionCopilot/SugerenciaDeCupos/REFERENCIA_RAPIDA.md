# ⚡ REFERENCIA RÁPIDA - ObtenerNegocios

## 1️⃣ QUÉ HACE EN 30 SEGUNDOS
El método `ObtenerNegocios` es un algoritmo que:
1. Trae contratos finalizados de la BD
2. Calcula cupos basado en kilos disponibles (SAP)
3. Aplica 4 descuentos sucesivos
4. Retorna sugerencias viables + no viables

---

## 2️⃣ LOS 4 DESCUENTOS (ORDEN IMPORTA)

| #  | Qué                    | Dónde                         | Por Qué                |
|----|------------------------|-------------------------------|------------------------|
| 1️⃣ | Cupos Pendientes       | BD: `Cupo`                    | Ya ocupan espacio      |
| 2️⃣ | Solicitudes Pendientes | BD: `AdministracionCupo`      | También ocupan espacio |
| 3️⃣ | CCPP Pendientes        | SAP: `CartasDePortePendiente` | Mercadería flotante    |
| 4️⃣ | Stock Disponible       | SAP: Establecimientos         | Límite físico          |

**Fórmula**:
```
CantidadDeCupos = CEILING(KgPendiente / 30000)
                  - CuposPendientes
                  - SolicitudesPendientes
                  - CCPPPendientes
                  - (si Sustentable/EPA) StockLimitante
```

---

## 3️⃣ VALIDACIONES PARA TU MÉTODO (desde Excel)

### ✅ OBLIGATORIAS
- [ ] Obtener `ContratoSAP` desde Excel
- [ ] Lookup del Contrato en BD
- [ ] Consultar `KgPendienteAplicar` en SAP
- [ ] Validar contra `ConfiguracionCupo` (límites)
- [ ] Validar Cupos Pendientes (BD)
- [ ] Validar CCPP Pendientes (SAP)
- [ ] Validar Stock si Sustentable/EPA/EUDR

### ❌ NO NECESARIAS
- Filtrar EPA/EUDR/Fason (entrada ya validada)
- Procesar Espacio Dinámico
- Ordenar por prioridad (usuario lo hace)

---

## 4️⃣ CAMPOS CRÍTICOS

```csharp
// INPUT (Excel)
ContratoSAP            // Identificador principal
CantidadDeCupos        // Cantidad a sugerir (validar)
FechaSugerida          // Fecha de la sugerencia

// LOOKUP (BD)
NegocioId              // ID del contrato
ProveedorId            // ID del proveedor
ComercialId            // ID del comercial
CentroId               // Centro de descarga
MaterialId             // Material

// CÁLCULO (SAP)
KgPendienteAplicar     // Kilos disponibles en SAP
KgNegocio              // Kilos totales del contrato

// DESCUENTOS
CuposPendientes        // Cupos recientes no completados
SolicitudesPendientes  // Solicitudes esperando resolución
CCPPDescuento          // Cupos por CCPP pendientes
StockDisponible        // Stock en establecimientos

// OUTPUT
Inhabilitado           // Razones de limitación
```

---

## 5️⃣ MÉTODOS SAP PRINCIPALES

### ContratoKgPendienteAgent.Consultar()
```csharp
INPUT:  List<ContratoSAP>
OUTPUT: List<KgPendiente>
USA:    Para obtener kilos disponibles
```

### CartasDePortePendienteAplicarAgent.ListarCartasDePortePendienteAplicar()
```csharp
INPUT:  CentroSap, MaterialCodigo, ProveedorCUIT, CorredorCUIT
OUTPUT: List<CCPP>
USA:    Para obtener cartas de porte pendientes
```

---

## 6️⃣ VALIDACIONES POR CAMPO

| Campo           | Validar Contra          | Error Si...       |
|-----------------|-------------------------|-------------------|
| ContratoSAP     | Negocio.ContratoSAP     | No existe         |
| KgPendiente     | >= kilosMinimos         | Insuficientes     |
| CantidadDeCupos | <= StockDisponible      | Supera stock      |
| FechaSugerida   | ConfiguracionCupo.Fecha | Fuera de rango    |
| CentroId        | Centro activo           | Centro inactivo   |
| MaterialId      | Material activo         | Material inactivo |

---

## 7️⃣ ORDEN DE IMPLEMENTACIÓN

### Paso 1: Validación Básica
```csharp
1. Existe ContratoSAP?
2. Existe Centro/Material?
3. CantidadDeCupos > 0?
```

### Paso 2: Obtener Datos
```csharp
4. Lookup Negocio por ContratoSAP
5. Obtener KgPendienteAplicar (SAP)
6. Obtener ConfiguracionCupo
```

### Paso 3: Validar Límites
```csharp
7. Validar total vs ConfiguracionCupo.LimiteAlgoritmo
8. Validar KgPendiente >= mínimo
9. Validar CantidadDeCupos <= StockDisponible
```

### Paso 4: Validar Ocupación
```csharp
10. Restar Cupos Pendientes
11. Restar Solicitudes Pendientes
12. Restar CCPP Pendientes
13. Restar Stock Disponible (si especial)
```

### Paso 5: Guardar
```csharp
14. Crear SugerenciaCupo con datos finales
15. Registrar razones en Inhabilitado
```

---

## 8️⃣ CONFIGURACIÓN CLAVE

```csharp
// Obtener del repositorio
Configuracion config = repositorio.Obtener<Configuracion>(1);
int kilosMinimos = config.AlgoritmoKilosMinimosParaSugerencia * 1000;

// Obtener del repositorio (por fecha/centro/material)
ConfiguracionCupo config = repositorio.Obtener<ConfiguracionCupo>(
    x => x.CentroId == centroId && 
         x.MaterialId == materialId &&
         x.Fecha == fecha
);
int limiteCupo = config.LimiteCupo;
int limiteAlgoritmo = config.LimiteAlgoritmo;
```

---

## 9️⃣ CÓDIGOS IMPORTANTES

```csharp
// Estados de Cupo
EnumEstadoCupo.Anulado      // -1
EnumEstadoCupo.Rechazado    // -2
EnumEstadoCupo.SinCTG       // 0

// Estados de Contrato
EnumEstadoContrato.Finalizado // 5

// Tipos de Administración
EnumTipoAdministracionCupo.Algoritmo

// Estados de Administración
EnumEstadoAdministracionCupo.Pendiente
EnumEstadoAdministracionCupo.Aceptado
```

---

## 🔟 ERRORES COMUNES A EVITAR

❌ **No consultar KgPendiente en SAP**
→ Usarás cantidad inicial incorrecta

❌ **Aplicar descuentos en orden incorrecto**
→ Cálculo incorrecto de cantidad final

❌ **Olvidar validar stock Sustentable**
→ Sugerencias que no pueden procesarse

❌ **No revisar ConfiguracionCupo**
→ Superar límites configurados

❌ **No acumular razones en Inhabilitado**
→ Usuario no sabe por qué se limitó

---

## 1️⃣1️⃣ PRUEBA RÁPIDA

Responde sin mirar atrás:

1. ¿Cuáles son los 4 descuentos? 
   → Pendientes, Solicitudes, CCPP, Stock

2. ¿De dónde viene KgPendiente?
   → De SAP (ContratoKgPendienteAgent)

3. ¿Cuántos kilos = 1 cupo?
   → 30,000 kg

4. ¿Cuándo se valida stock?
   → Al final (descuento 4)

5. ¿Dónde se valida ConfiguracionCupo?
   → Al inicio (antes de validaciones)

**Si respondiste todo correctamente → Ya puedes implementar**

---

## 1️⃣2️⃣ ESTRUCTURA DE RETORNO

```csharp
public class Resultado
{
    public bool HayError { get; set; }
    public List<string> Errores { get; set; }  // Contratos no válidos
    public List<SugerenciaCupo> Sugerencias { get; set; }  // Válidos
}

// Uso
if (resultado.HayError)
{
    // Mostrar errores al usuario
    // No guardar nada
}
else
{
    // Guardar sugerencias
    repositorio.AgregarTodos(resultado.Sugerencias);
}
```

---

## 1️⃣3️⃣ LINKS A DOCUMENTOS COMPLETOS

- **Entendimiento**: ANALISIS_OBTENER_NEGOCIOS.md
- **Visualización**: ESQUEMA_VISUAL_FLUJO.md
- **Implementación**: RECOMENDACIONES_METODO_MANUAL.md
- **Código**: OBTENER_NEGOCIOS_COMENTADO_PARTE[1-3].cs

---

**RESUMEN**: 4 descuentos, 5 validaciones, 2 consultas SAP = Tu método completo

