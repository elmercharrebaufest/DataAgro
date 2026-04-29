# 📋 Documentación Generada - Análisis de `ObtenerNegocios`

## 📁 Archivos Creados

### 1. **ANALISIS_OBTENER_NEGOCIOS.md** ⭐ LEER PRIMERO
**Propósito**: Introducción y análisis ejecutivo del método  
**Contiene**:
- Propósito general del método
- Flujo de ejecución en 6 fases principales
- Tabla con campos importantes en `SugerenciaCupoDto`
- Validaciones clave
- Recomendaciones para crear un método similar desde Excel

**ACCIÓN**: Lee esto primero para entender el contexto global.

---

### 2. **OBTENER_NEGOCIOS_COMENTADO_PARTE1.cs**
**Propósito**: Versión comentada del método (Fases 1-7)  
**Contiene**:
- FASE 1: Obtención de exclusiones
- FASE 2: Consulta principal de contratos (con WHERE comentado)
- FASE 3: Obtención de datos auxiliares
- FASE 4: Obtener cupos pendientes
- FASE 5: Obtener solicitudes pendientes
- FASE 6: Consultar kilos pendientes en SAP
- FASE 7: Calcular cantidad de cupos (con descuentos 1 y 2)

**Características**:
- Comentarios muy detallados (casi línea por línea)
- Explica el "por qué" de cada operación
- Ideal para entender la lógica de cálculo de cupos

---

### 3. **OBTENER_NEGOCIOS_COMENTADO_PARTE2.cs**
**Propósito**: Versión comentada del método (Fases 8-11)  
**Contiene**:
- FASE 8: Validación de Cartas de Porte Pendientes (CCPP)
- FASE 9: Aplicación de descuentos de CCPP a contratos
- FASE 10: Gestión de stock Sustentable
- FASE 11: Gestión de stock EPA/EUDR

**Características**:
- Explicación detallada de cómo se calculan cupos para CCPP
- Cómo se ordenan contratos por prioridad
- Cómo se valida stock disponible en establecimientos

---

### 4. **OBTENER_NEGOCIOS_COMENTADO_PARTE3.cs**
**Propósito**: Versión comentada del método (Fases 12-14 y RETORNO)  
**Contiene**:
- FASE 12: Procesamiento de Espacio Dinámico
- FASE 13: Agregación de Espacio Dinámico a lista final
- FASE 14: Retorno con negociosSinSugerencia

**Características**:
- Explicación de Espacio Dinámico
- Cómo se finaliza el procesamiento
- Definición del valor de retorno

---

### 5. **ESQUEMA_VISUAL_FLUJO.md** ⭐ RECOMENDADO PARA VISUALIZAR
**Propósito**: Diagrama visual del flujo completo  
**Contiene**:
- Diagrama ASCII del flujo de ejecución
- Estructura de decisiones (if/then/else)
- Orden de descuentos
- Mapeo de campos importantes

**ACCIÓN**: Abre este archivo con un visualizador markdown. Es muy visual y fácil de seguir.

---

### 6. **RECOMENDACIONES_METODO_MANUAL.md** ⭐ LEER ANTES DE IMPLEMENTAR
**Propósito**: Guía para crear el método manual desde Excel  
**Contiene**:
- Resumen comparativo (Automático vs Manual)
- Validaciones necesarias (obligatorias vs opcionales)
- Código ejemplo del flujo sugerido
- Tabla de campos que debes completar
- Recomendación de arquitectura (2 métodos)

**ACCIÓN**: Lee esto cuando estés listo para implementar el nuevo método.

---

## 🎯 Recomendación de Lectura Sugerida

### Para entender `ObtenerNegocios`:
1. **ANALISIS_OBTENER_NEGOCIOS.md** (5 min)
2. **ESQUEMA_VISUAL_FLUJO.md** (10 min)
3. **OBTENER_NEGOCIOS_COMENTADO_PARTE1.cs** (15 min)
4. **OBTENER_NEGOCIOS_COMENTADO_PARTE2.cs** (15 min)
5. **OBTENER_NEGOCIOS_COMENTADO_PARTE3.cs** (10 min)

**Total**: ~55 minutos para una comprensión completa

---

### Para implementar el método manual:
1. **RECOMENDACIONES_METODO_MANUAL.md** (especialmente la sección "Validaciones Necesarias")
2. **OBTENER_NEGOCIOS_COMENTADO_PARTE1.cs** (sección 5: Cálculo de cupos)
3. **OBTENER_NEGOCIOS_COMENTADO_PARTE2.cs** (secciones 8-11: Validaciones)

---

## 🔑 Conceptos Clave Identificados

### 1. **Orden de Descuentos**
El algoritmo aplica descuentos en este orden:
1. Cupos pendientes del ciclo anterior
2. Solicitudes de administración pendientes
3. CCPP pendientes (por proveedor/corredor)
4. Stock disponible (si es Sustentable/EPA/EUDR)

### 2. **Tres Niveles de Validación**
- **Nivel 1**: Filtrado de contratos (BD)
- **Nivel 2**: Validación de disponibilidad (kilos, cupos pendientes, solicitudes)
- **Nivel 3**: Validación de stock físico (sustentables, EPA, EUDR)

### 3. **Campos Críticos**
- `KgPendienteAplicar`: Consultado en SAP (es el dato real)
- `CantidadDeCupos`: Recalculado varias veces (no es lineal)
- `Inhabilitado`: Se acumula con razones de limitación

### 4. **Dos Tipos de Negocios**
- **Contratos Regulares**: De la BD, con ContratoSAP
- **Espacio Dinámico**: Configuraciones alternativas, sin contrato

---

## 📊 Estadísticas del Código

| Métrica                   | Valor |
|---------------------------|-------|
| Líneas totales del método | 455   |
| Fases distintas           | 14    |
| Consultas a BD            | 7     |
| Llamadas a agentes SAP    | 2     |
| Loops/Bucles              | 8     |
| Descuentos aplicados      | 4     |
| Validaciones de tipo      | 3     |

---

## ❓ Preguntas Frecuentes

### P: ¿Por qué se consulta SAP dos veces en el método automático?
**R**: 
1. `ContratoKgPendienteAgent.Consultar()`: Para obtener kilos reales disponibles
2. `CartasDePortePendienteAplicarAgent.ListarCartasDePortePendiente()`: Para obtener CCPP pendientes

Son datos diferentes necesarios para diferentes validaciones.

### P: ¿Qué es `Inhabilitado`?
**R**: Es un campo string que acumula razones por las cuales un cupo fue limitado o descartado. Ej: "Posee 5 cupos pendientes. Por CP pendiente a aplicar, se restan 2 cupos. Tiene 3 cupos SUST pendientes..."

### P: ¿Por qué se procesan Sustentables y EPA/EUDR por separado?
**R**: Tienen restricciones diferentes. Sustentables se limitan a stock en ciertos establecimientos, EPA/EUDR se limitan a stock en establecimientos especiales de EPA/EUDR.

### P: ¿Qué hace `TraerCuposDisponiblesEnEstablecimientos()`?
**R**: Suma cupos disponibles en todos los establecimientos del proveedor, validando que cada establecimiento tenga >= 28,000 kg.

---

## 🚀 Próximos Pasos

### Para integrar comentarios en el código actual:
```
// Opción 1: Hacerlo gradualmente
// Aplicar comentarios fase por fase mientras se testean cambios

// Opción 2: Crear versión paralela
// Dejar método actual intacto, crear ObtenerNegociosComentado()

// Opción 3: Refactorizar con métodos privados
// Extraer cada fase a método privado con nombre descriptivo:
//   - ObtenerContratosCandidatos()
//   - CalcularCuposPorContrato()
//   - AplicarDescuentosCCPP()
//   - ValidarStockSustentable()
//   - ProcesarEspacioDinamico()
```

---

## 📞 Contexto de la Solicitud Original

El usuario requería:
1. ✅ Análisis del método `ObtenerNegocios`
2. ✅ Comentarios detallados
3. ✅ Un archivo .md con explicación
4. ✅ Preparación para crear un nuevo método que permita sugerencias manuales desde Excel

Los cambios principales para el método manual:
- Entrada: Excel en lugar de consulta BD
- Validaciones: Se mantienen las mismas
- Salida: `SugerenciaCupo` guardadas con validaciones aplicadas

---

## 📝 Notas Finales

- Todo el código está documentado en C# 7.3 / .NET Framework 4.7.2 (según la especificación del proyecto)
- Los comentarios están en español para consistencia con el codebase
- Se han preservado las lógicas de negocio tal como existen
- Las recomendaciones para el nuevo método son sugerencias basadas en el patrón existente

