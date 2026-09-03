# Ejemplos de Uso - Auditoría de Contacto Comercial

## Escenario 1: Ver todos los cambios de un Contacto Comercial

### Pasos:
1. Navegue a `/ContactoComercialAuditoria/Index`
2. En "ID Contacto Comercial", ingrese `646`
3. Haga clic en el botón "Buscar Historial" en la sección "Historial de Contacto"
4. Se cargará la grilla con todos los cambios realizados a ese contacto

### Resultado esperado:
- Grilla muestra múltiples filas (si hay cambios)
- Columnas visibles: ID, Apellido, Nombres, Operación, Usuario, Fecha, Valores Anteriores, Valores Nuevos
- Puede hacer clic en las columnas para ordenar
- Puede escribir en el filtro para buscar por contenido
- Puede navegar entre páginas si hay muchos registros

### Ejemplo de dato mostrado:
```
ID: 646
Apellido: López
Nombres: Juan
Operación: UPDATE
Usuario: gsian
Fecha: 15/03/2024 14:30:45
Valores Anteriores: Apellido: Lopez, Nombres: Juan José
Valores Nuevos: Apellido: López, Nombres: Juan José
```

---

## Escenario 2: Ver cambios realizados en los últimos 7 días

### Pasos:
1. Navegue a `/ContactoComercialAuditoria/Index`
2. En "Días (Cambios Recientes)", asegúrese de que tenga `7`
3. Haga clic en el botón "Buscar Cambios Recientes" en la sección "Cambios Recientes"
4. Se cargará la grilla con cambios recientes del sistema

### Resultado esperado:
- Grilla muestra todos los cambios de ContactosComerciales en los últimos 7 días
- Muestra múltiples contactos diferentes
- Utilidades: descubrir quién ha estado modificando contactos recientemente

### URL directa:
```
/ContactoComercialAuditoria/CambiosRecientes?dias=14
```
(Para los últimos 14 días)

---

## Escenario 3: Ver cambios de un usuario específico

### Pasos:
1. Navegue a `/ContactoComercialAuditoria/Index`
2. En "Usuario", ingrese `gsian`
3. Haga clic en el botón "Buscar Cambios por Usuario" en la sección "Cambios por Usuario"
4. Se cargará la grilla con todos los cambios realizados por ese usuario

### Resultado esperado:
- Grilla muestra cambios de múltiples contactos hechos por `gsian`
- Útil para auditoría de usuario específico
- Puede filtrar y ordenar por contacto, fecha, etc.

### Caso de uso:
```
Necesito verificar qué cambios realizó el usuario "jsmith" el mes pasado
1. Ingrese "jsmith" en Usuario
2. Haga clic en "Buscar Cambios por Usuario"
3. Revise todos los cambios realizados por jsmith
```

---

## Escenario 4: Verificar el último cambio de un contacto

### Pasos:
1. Navegue a `/ContactoComercialAuditoria/Index`
2. En "ID Contacto Comercial", ingrese `123`
3. Haga clic en el botón "Buscar Último Cambio" en la sección "Último Cambio"
4. Se cargará una grilla con una sola fila (el último cambio)

### Resultado esperado:
- Grilla muestra una sola fila
- Contiene el cambio más reciente del contacto 123
- Útil para verificar cuándo fue la última modificación

### Información mostrada:
```
ID: 123
Apellido: García
Nombres: María
Operación: UPDATE
Usuario: asanchez
Fecha: 20/03/2024 10:15:30
```

---

## Escenario 5: Ver resumen de operaciones de un contacto

### Pasos:
1. Navegue a `/ContactoComercialAuditoria/Index`
2. En "ID Contacto Comercial", ingrese `646`
3. Haga clic en el botón "Buscar Resumen" en la sección "Resumen por Tipo de Operación"
4. Se cargará la grilla con el resumen

### Resultado esperado:
- Grilla muestra 3 filas (INSERT, UPDATE, DELETE)
- Cada fila contiene:
  - Tipo de Operación (INSERT, UPDATE o DELETE)
  - Cantidad de cambios de ese tipo
  - Última modificación (última vez que se realizó ese tipo de operación)

### Ejemplo de resultado:
```
Tipo de Operación: INSERT
Cantidad: 1
Última Modificación: 01/01/2024 09:00:00

Tipo de Operación: UPDATE
Cantidad: 15
Última Modificación: 20/03/2024 10:15:30

Tipo de Operación: DELETE
Cantidad: 0
Última Modificación: (NULL)
```

### Interpretación:
- El contacto fue creado una vez (INSERT)
- Ha sido modificado 15 veces
- Nunca ha sido eliminado

---

## Escenario 6: Limpiar todos los datos

### Pasos:
1. Navegue a `/ContactoComercialAuditoria/Index`
2. Haga búsquedas en varias secciones (las grillas se llenan de datos)
3. Haga clic en "Limpiar Todo" en la sección de Filtros Generales
4. Todas las grillas se vaciarán

### Resultado esperado:
- Las 5 grillas quedan vacías
- Puede comenzar nuevas búsquedas
- Los filtros se mantienen (ID, Usuario, Días)

---

## Escenario 7: Usar URLs directas para acceso rápido

### URLs de ejemplo:

**Ver historial del contacto 100:**
```
/ContactoComercialAuditoria/Historial?contactoComercialId=100
```

**Ver cambios de los últimos 30 días:**
```
/ContactoComercialAuditoria/CambiosRecientes?dias=30
```

**Ver cambios del usuario admin:**
```
/ContactoComercialAuditoria/CambiosPorUsuario?usuario=admin
```

**Ver último cambio del contacto 999:**
```
/ContactoComercialAuditoria/UltimoCambio?contactoComercialId=999
```

**Ver resumen del contacto 500:**
```
/ContactoComercialAuditoria/ResumenPorTipoOperacion?contactoComercialId=500
```

---

## Escenario 8: Interpretar los datos de auditoría

### Formato de "Valores Anteriores" y "Valores Nuevos"

Ejemplo real:
```
Valores Anteriores: ContactoComercialId: 646, ProveedorId: 1, Apellido: López, Nombres: Juan, 
                    Email1: juan@example.com, Puesto: Gerente, Cargo: Comercial

Valores Nuevos: ContactoComercialId: 646, ProveedorId: 1, Apellido: López, Nombres: Juan José, 
                Email1: juan.nuevo@example.com, Puesto: Gerente Senior, Cargo: Comercial
```

### Interpretación:
- Solo 3 campos cambiaron: `Nombres`, `Email1` y `Puesto`
- El resto de campos permaneció igual
- Se puede rastrear exactamente qué cambió y cuándo

---

## Troubleshooting de Escenarios

### Problema: No hay datos en la grilla
**Solución:**
1. Verificar que el ID de contacto existe
2. Verificar que el usuario existe
3. Verificar que hay cambios en el período especificado
4. Revisar la consola del navegador para errores AJAX

### Problema: Los valores anteriores/nuevos aparecen truncados
**Solución:**
1. Hacer hover sobre la celda para ver el valor completo en tooltip
2. Ampliar la ventana del navegador
3. Hacer clic en la celda (si se implementa expanded view en futuro)

### Problema: Las grillas se cargan lentamente
**Solución:**
1. Reducir el rango de búsqueda (menos días, contacto específico)
2. Usar filtros más específicos
3. Contactar al administrador si el problema persiste

---

## Casos de Uso Reales

### Caso 1: Auditoría de cambios
```
Necesito verificar todos los cambios hechos al contacto comercial 646 para crear un reporte.

Acción:
1. Ir a /ContactoComercialAuditoria/Index
2. ID Contacto: 646
3. Clic en "Buscar Historial"
4. Revisar el Historial completo
5. Exportar (funcionalidad futura) o tomar captura de pantalla
```

### Caso 2: Investigación de cambios recientes
```
El contacto 123 tiene datos incorrectos. Necesito saber quién y cuándo lo modificó.

Acción:
1. Ir a /ContactoComercialAuditoria/Index
2. ID Contacto: 123
3. Clic en "Buscar Último Cambio"
4. Ver quién lo modificó y cuándo
5. Contactar a ese usuario si es necesario
```

### Caso 3: Seguimiento de usuario
```
Necesito revisar todas las modificaciones realizadas por "mgarcia" en los últimos 30 días.

Acción:
1. Ir a /ContactoComercialAuditoria/Index
2. Usuario: mgarcia
3. Días: 30
4. Clic en "Buscar Cambios por Usuario"
5. Revisar todos los cambios realizados
```

### Caso 4: Análisis de actividad
```
Necesito ver un resumen de qué tipos de cambios se hacen en cada contacto.

Acción:
1. Ir a /ContactoComercialAuditoria/Index
2. Para cada contacto de interés:
   - ID Contacto: [número]
   - Clic en "Buscar Resumen"
   - Analizar cantidad de INSERT/UPDATE/DELETE
```

---

## Atajos Útiles

### Para desarrolladores/administradores
```
# Ver cambios del último mes
/ContactoComercialAuditoria/CambiosRecientes?dias=30

# Ver cambios por usuario específico para auditoría
/ContactoComercialAuditoria/CambiosPorUsuario?usuario=nombreusuario

# Ver historial completo de un contacto problemático
/ContactoComercialAuditoria/Historial?contactoComercialId=ID_PROBLEMA
```

---

## Análisis de Rendimiento

La pantalla está optimizada para:
- ✅ Consultas rápidas (< 2 segundos en datos normales)
- ✅ Grillas con paginación (solo carga lo visible + algo más)
- ✅ Filtros cliente-side (rápidos y sin servidor)
- ✅ Ordenamiento cliente-side (instantáneo)

Para búsquedas muy grandes, considere:
- Reducir el rango de fechas
- Usar filtros más específicos
- Contactar al administrador de BD
