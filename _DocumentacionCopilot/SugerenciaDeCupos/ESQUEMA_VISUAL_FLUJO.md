# Esquema Visual del Flujo en `ObtenerNegocios`

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      INICIO: ObtenerNegocios()                          │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ FASE 1: OBTENER EXCLUSIONES                           │
        ├───────────────────────────────────────────────────────┤
        │ • Tipos de negocio excluidos (por fórmula)            │
        │ • Contratos con sustituto (AnulaYReemplaza)           │
        └───────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ FASE 2: CONSULTA PRINCIPAL DE CONTRATOS               │
        ├───────────────────────────────────────────────────────┤
        │ WHERE:                                                │
        │   ✗ !EPA AND !EUDR AND !Fason                        │
        │   ✗ !TipoNegocioExcluido                             │
        │   ✗ !TieneAnulaYReemplaza                            │
        │   ✓ Estado = Finalizado                               │
        │   ✓ Centro = formula.CentroId                         │
        │   ✓ Material = formula.MaterialId                     │
        │   ✓ Fecha BETWEEN NegociosDesde/NegociosHasta         │
        │                                                       │
        │ MAPEO: Contrato → SugerenciaCupoDto                   │
        │   - Cantidad inicial = CEILING(Kg / 30000)            │
        │   - Todos los datos comerciales                       │
        │   - KgPendienteAplicar = 0 (por ahora)                │
        └───────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ FASE 3: OBTENER DATOS AUXILIARES                      │
        ├───────────────────────────────────────────────────────┤
        │ • Zonas de cupo configuradas                          │
        │ • Mínimo de kilos para sugerencia (config)            │
        │ • Cupos pendientes (generados recientes)              │
        │ • Solicitudes pendientes (administración)             │
        └───────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ FASE 4: CONSULTAR KILOS PENDIENTES (SAP)              │
        ├───────────────────────────────────────────────────────┤
        │ ContratoKgPendienteAgent.Consultar()                  │
        │   INPUT:  List<ContratoSAP>                           │
        │   OUTPUT: List<KgPendiente>                           │
        └───────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ╔═══════════════════════════════════════════════════════╗
        ║ FASE 5: LOOP - PROCESAR CADA CONTRATO                 ║
        ╠═══════════════════════════════════════════════════════╣
        ║                                                       ║
        ║  POR CADA item IN contratos:                          ║
        ║  ┌─────────────────────────────────────────────┐      ║
        ║  │ 5.1: Asignar ZonaCupoId                     │      ║
        ║  │      zona.Descripcion == item.ZonaDescrip   │      ║
        ║  └─────────────────────────────────────────────┘      ║
        ║           │                                           ║
        ║           ▼                                           ║
        ║  ┌─────────────────────────────────────────────┐      ║
        ║  │ 5.2: Obtener KgPendiente de SAP             │      ║
        ║  │      item.KgPendienteAplicar = KgPendiente  │      ║
        ║  └─────────────────────────────────────────────┘      ║
        ║           │                                           ║
        ║           ▼                                           ║
        ║  ┌─────────────────────────────────────────────┐      ║
        ║  │ 5.3: CALCULAR CUPOS                         │      ║
        ║  │                                             │      ║
        ║  │ if KgPendiente >= kilosMinimos              │      ║
        ║  │   ├─ cuposBase = FLOOR(Kg / 30000)          │      ║
        ║  │   ├─ excedente = FRAC(Kg/30000) * 100       │      ║
        ║  │   └─ if excedente >= mínimo THEN +1 cupo    │      ║
        ║  │ else                                        │      ║
        ║  │   └─ CantidadDeCupos = 0 ❌                 │      ║
        ║  └─────────────────────────────────────────────┘      ║
        ║           │                                           ║
        ║           ▼                                           ║
        ║  ┌─────────────────────────────────────────────┐      ║
        ║  │ 5.4: DESCUENTO 1 - Cupos Pendientes         │      ║
        ║  │                                             │      ║
        ║  │ if item.NegocioId IN cuposPendientes        │      ║
        ║  │   ├─ CantidadDeCupos -= pendientes          │      ║
        ║  │   └─ Inhabilitado += mensaje                │      ║
        ║  └─────────────────────────────────────────────┘      ║
        ║           │                                           ║
        ║           ▼                                           ║
        ║  ┌─────────────────────────────────────────────┐      ║
        ║  │ 5.5: DESCUENTO 2 - Solicitudes Pendientes   │      ║
        ║  │                                             │      ║
        ║  │ if item.NegocioId IN solicitudesPendientes  │      ║
        ║  │   ├─ CantidadDeCupos -= solicitudes         │      ║
        ║  │   └─ Inhabilitado += mensaje                │      ║
        ║  └─────────────────────────────────────────────┘      ║
        ║                                                       ║
        ╚═══════════════════════════════════════════════════════╝
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ FASE 6: SEPARAR VIABLES vs NO VIABLES                 │
        ├───────────────────────────────────────────────────────┤
        │ negociosSinSugerencia += donde CantidadDeCupos <= 0   │
        │ negocios += donde CantidadDeCupos > 0                 │
        └───────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ FASE 7: VALIDAR WARRANTY/CD                           │
        ├───────────────────────────────────────────────────────┤
        │ cdWarrant.ConsultarContratoWarrant()                  │
        │   INPUT:  Rango de fechas                             │
        │   OUTPUT: List<ContratoSAP con Warranty>              │
        │                                                       │
        │ FOR EACH contrato:                                    │
        │   if ContratoSAP encontrado en lista                  │
        │     THEN CDWarrant = true                             │
        └───────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ╔═══════════════════════════════════════════════════════╗
        ║ FASE 8: VALIDAR CCPP PENDIENTES                       ║
        ╠═══════════════════════════════════════════════════════╣
        ║                                                       ║
        ║  GROUP contratos BY (CUITProveedor, CUITCorredor)     ║
        ║                                                       ║
        ║  FOR EACH grupo:                                      ║
        ║    ┌──────────────────────────────────────────┐       ║
        ║    │ 8.1: Llamar CartasDePortePendiente       │       ║
        ║    │      INPUT:  Centro/Material/Prov/Corr   │       ║
        ║    │      OUTPUT: List<CCPP pendientes>       │       ║
        ║    └──────────────────────────────────────────┘       ║
        ║              │                                        ║
        ║              ▼                                        ║
        ║    ┌──────────────────────────────────────────┐       ║
        ║    │ 8.2: if cantidadCCPP >= kilosMinimos     │       ║
        ║    │      ├─ Calcular cupos equivalentes      │       ║
        ║    │      └─ Guardar en restarCuposProvCorr   │       ║
        ║    └──────────────────────────────────────────┘       ║
        ║                                                       ║
        ╚═══════════════════════════════════════════════════════╝
                                    │
                                    ▼
        ╔═════════════════════════════════════════════════════╗
        ║ FASE 9: APLICAR DESCUENTOS CCPP A CONTRATOS         ║
        ╠═════════════════════════════════════════════════════╣
        ║                                                     ║
        ║  ORDENA contratos BY:                               ║
        ║    - Canje DESC                                     ║
        ║    - CDWarrant DESC                                 ║
        ║    - MercsDeposito DESC                             ║
        ║    - FechaDesde ASC                                 ║
        ║    - ContratoSAP ASC                                ║
        ║                                                     ║
        ║  FOR EACH contratoOrdenado:                         ║
        ║    ┌──────────────────────────────────────────┐     ║
        ║    │ 9.1: Obtener cupos a restar              │     ║
        ║    │      restarCuposProvCorr[prov,corr]      │     ║
        ║    └──────────────────────────────────────────┘     ║
        ║         │                                           ║
        ║         ▼                                           ║
        ║    ┌──────────────────────────────────────────┐     ║
        ║    │ 9.2: if tieneEnoughCupos                 │     ║
        ║    │      ├─ THEN descuenta todo              │     ║
        ║    │      └─ ELSE descuenta lo que puede      │     ║
        ║    └──────────────────────────────────────────┘     ║
        ║         │                                           ║
        ║         ▼                                           ║
        ║    ┌──────────────────────────────────────────┐     ║
        ║    │ 9.3: Actualiza lista y agrega mensaje    │     ║
        ║    │      Inhabilitado += motivo              │     ║
        ║    └──────────────────────────────────────────┘     ║
        ║                                                     ║
        ╚═════════════════════════════════════════════════════╝
                                    │
                                    ▼
        ╔═════════════════════════════════════════════════════╗
        ║ FASE 10: GESTIÓN STOCK SUSTENTABLE                  ║
        ╠═════════════════════════════════════════════════════╣
        ║                                                     ║
        ║  FILTER contratos WHERE Sustentable = true          ║
        ║                                                     ║
        ║  FOR EACH proveedor con Sustentables:               ║
        ║    ┌──────────────────────────────────────────┐     ║
        ║    │ 10.1: Obtener stock en establecimientos  │     ║
        ║    │       TraerCuposDisponiblesEnEstab(…)    │     ║
        ║    └──────────────────────────────────────────┘     ║
        ║         │                                           ║
        ║         ▼                                           ║
        ║    ┌──────────────────────────────────────────┐     ║
        ║    │ 10.2: Descontar cupos pendientes         │     ║
        ║    │       stock -= cuposPendientesSust       │     ║
        ║    └──────────────────────────────────────────┘     ║
        ║         │                                           ║
        ║         ▼                                           ║
        ║    ┌──────────────────────────────────────────┐     ║
        ║    │ 10.3: Limitar cada contrato a stock      │     ║
        ║    │       if CantidadDeCupos > stockDisp     │     ║
        ║    │       ├─ CantidadDeCupos = stockDisp     │     ║
        ║    │       └─ Inhabilitado += mensaje         │     ║
        ║    └──────────────────────────────────────────┘     ║
        ║                                                     ║
        ╚═════════════════════════════════════════════════════╝
                                    │
                                    ▼
        ╔═══════════════════════════════════════════════════════╗
        ║ FASE 11: GESTIÓN STOCK EPA/EUDR                       ║
        ║ (IGUAL QUE FASE 10, PERO CON esEPAoEUDR=true)         ║
        ╚═══════════════════════════════════════════════════════╝
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ FASE 12: PROCESAMIENTO ESPACIO DINÁMICO               │
        ├───────────────────────────────────────────────────────┤
        │ • Obtiene configuraciones de "Espacio Dinámico"       │
        │ • Aplica mismas validaciones que contratos            │
        │ • Agrega a lista final si hay cupos disponibles       │
        └───────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌───────────────────────────────────────────────────────┐
        │ RETORNO                                               │
        ├───────────────────────────────────────────────────────┤
        │ • negociosSinSugerencia (CantidadDeCupos <= 0)        │
        │ • negocios (por referencia) += contratos viables      │
        │ • Ambas listas con todos los descuentos aplicados     │
        └───────────────────────────────────────────────────────┘
```

---

## Decisiones Clave en el Flujo

### 1. **Orden de Descuentos (IMPORTANTE)**
```
CantidadDeCupos = CEILING(Kg / 30000)
         │
         ├─ DESCUENTO 1: Cupos Pendientes
         │
         ├─ DESCUENTO 2: Solicitudes Pendientes
         │
         ├─ DESCUENTO 3: CCPP Pendientes (proporcional)
         │
         └─ DESCUENTO 4: Stock Disponible (Sustentable/EPA/EUDR)
```

El orden importa porque algunos descuentos dependen de otros.

### 2. **Cupos Pendientes vs Solicitudes Pendientes**
- **Cupos Pendientes**: Ya fueron creados y enviados a SAP
- **Solicitudes Pendientes**: Fueron solicitados pero aún no creados

Ambos "ocupan" cupos del negocio.

### 3. **CCPP Pendientes**
- Se descuentan **por proveedor/corredor**, no por contrato
- Se aplican **proporcionalmente** a contratos ordenados
- Si hay varias combinaciones proveedor/corredor, cada una tiene su propio descuento

### 4. **Stock Sustentable/EPA/EUDR**
- Es el **último filtro** (más restrictivo)
- Se valida contra establecimientos del proveedor
- Puede reducir cantidad a 0 aunque haya kilos disponibles

---

## Mapeo de Campos Importantes

| En Método | Corresponde a | Uso |
|-----------|---------------|-----|
| `formula` | `Formula` | Rangos de fechas y centro/material |
| `contratos` | `List<SugerenciaCupoDto>` | Principal working list |
| `KgPendiente` | De SAP | Base para calcular cupos |
| `kilosMinimosParaSugerencia` | Config global | Mínimo para sugerir |
| `minimo` | Cálculo interno | Threshold para redondeos |
| `ZonaCupoId` | Lookup por descripción | Zona de descarga |
| `Inhabilitado` | String acumulativo | Registro de limitaciones |
| `cuposPendientes` | Dict<NegocioId, Count> | Cuenta cupos pendientes |
| `solicitudesPendientes` | List<AdministracionCupo> | Solicitudes del algoritmo |
| `restarCuposProvCorr` | List DTO | Cupos reservados por CCPP |
| `stockSustentable` | List DTO | Stock en establecimientos |

