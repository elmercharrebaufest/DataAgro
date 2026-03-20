# Auditoría de Contacto Comercial - Guía de Uso

## Descripción General

Se ha creado una pantalla completa para visualizar los resultados de todos los métodos del `ContactoComercialAuditoriaController`. La solución incluye:

- **Vista CSHTML**: `WebDataAgro\Views\Home\ContactoComercialAuditoria.cshtml`
- **Script JavaScript**: `WebDataAgro\Scripts\App\ContactoComercialAuditoria.js`
- **Bundle**: Se agregó automáticamente en `BundleConfig.cs`
- **Acción en Controlador**: Método `Index()` en `ContactoComercialAuditoriaController.cs`

## Características

### 1. **Historial de Contacto**
- Busca el historial completo de cambios para un Contacto Comercial específico
- Muestra valores anteriores y nuevos de cada cambio
- Parámetro: ID del Contacto Comercial

### 2. **Cambios Recientes**
- Muestra todos los cambios realizados en los últimos N días
- Aplica a cualquier Contacto Comercial del sistema
- Parámetro: Número de días

### 3. **Cambios por Usuario**
- Filtra todos los cambios realizados por un usuario específico
- Muestra quién hizo qué cambios y cuándo
- Parámetro: Nombre de usuario

### 4. **Último Cambio**
- Obtiene el cambio más reciente para un Contacto Comercial específico
- Útil para verificar la última modificación
- Parámetro: ID del Contacto Comercial

### 5. **Resumen por Tipo de Operación**
- Agrupa cambios por tipo (INSERT, UPDATE, DELETE)
- Muestra cantidad de cambios y última modificación por tipo
- Parámetro: ID del Contacto Comercial

## Cómo Usar

### Acceso a la Pantalla
1. Navegar a: `/ContactoComercialAuditoria/Index`
2. O agregar un enlace en el menú de la aplicación

### Filtros Generales
- **ID Contacto Comercial**: Para búsquedas por contacto específico (default: 646)
- **Usuario**: Para búsquedas por usuario específico (default: gsian)
- **Días**: Para cambios recientes (default: 7)
- **Botón Limpiar Todo**: Borra todos los datos de las grillas

### Cada Sección
Cada sección tiene su propio botón de búsqueda que:
1. Valida los parámetros requeridos
2. Realiza la consulta al servidor
3. Carga los datos en la grilla correspondiente
4. Muestra un mensaje de error si hay problemas

## Funcionalidades de las Grillas (Kendo Grid)

- ✅ Ordenamiento de columnas
- ✅ Paginación
- ✅ Filtrado por contenido
- ✅ Scroll horizontal para columnas amplias
- ✅ Formato de fechas (dd/MM/yyyy HH:mm:ss)
- ✅ Truncado de valores largos con tooltip

## Parámetros de URL (Para URLs directas)

### Historial de Contacto
```
/ContactoComercialAuditoria/Historial?contactoComercialId=646
```

### Cambios Recientes
```
/ContactoComercialAuditoria/CambiosRecientes?dias=7
```

### Cambios por Usuario
```
/ContactoComercialAuditoria/CambiosPorUsuario?usuario=gsian
```

### Último Cambio
```
/ContactoComercialAuditoria/UltimoCambio?contactoComercialId=646
```

### Resumen por Operación
```
/ContactoComercialAuditoria/ResumenPorTipoOperacion?contactoComercialId=646
```

## Estructura de Datos Mostrada

### Columnas Generales
- **ID**: ID del Contacto Comercial
- **Apellido**: Apellido del contacto
- **Nombres**: Nombres del contacto
- **Operación**: Tipo de operación (INSERT, UPDATE, DELETE)
- **Usuario**: Usuario que realizó el cambio
- **Fecha**: Fecha y hora de la modificación

### Columnas Adicionales (en Historial)
- **Valores Anteriores**: Datos antes del cambio (formato: "Campo1: Valor1, Campo2: Valor2")
- **Valores Nuevos**: Datos después del cambio (mismo formato)

## Mejoras Realizadas

1. ✅ Interfaz limpia y consistente con el resto de la aplicación
2. ✅ Validación de parámetros en cliente
3. ✅ Manejo de errores con mensajes de alerta
4. ✅ Grillas con capacidades de filtrado, ordenamiento y paginación
5. ✅ Valores de auditoría en formato legible (campos separados por comas)
6. ✅ Tooltip para valores truncados
7. ✅ Diseño responsivo usando Bootstrap
8. ✅ Secciones colapsables (paneles)

## Notas Técnicas

- **Framework**: Kendo UI Grid
- **Validaciones**: Cliente-side
- **AJAX**: jQuery AJAX para llamadas al servidor
- **Formato de Fechas**: dd/MM/yyyy HH:mm:ss
- **Método HTTP**: GET para todas las consultas
- **JSON Response**: Estructura `{ data: [...] }`

## Troubleshooting

### No se carga la página
- Verificar que el bundle esté correctamente registrado en BundleConfig.cs
- Verificar que la ruta `/ContactoComercialAuditoria/Index` sea accesible

### Las grillas no muestran datos
- Verificar en la consola del navegador si hay errores AJAX
- Confirmar que el manager `IContactoComercialAuditoriaManager` está correctamente inyectado
- Validar que los parámetros sean válidos

### Valores de auditoría no se ven claros
- Los valores anteriores y nuevos están en formato legible: "NombreCampo: Valor, NombreCampo2: Valor2"
- Hacer hover sobre las celdas truncadas para ver el valor completo en el tooltip

## Próximas Mejoras (Opcional)

- Agregar botón para exportar a Excel
- Agregar gráficos de actividad por período
- Agregar más filtros avanzados
- Implementar búsqueda de rango de fechas
- Agregar comparador visual para ver antes/después lado a lado
