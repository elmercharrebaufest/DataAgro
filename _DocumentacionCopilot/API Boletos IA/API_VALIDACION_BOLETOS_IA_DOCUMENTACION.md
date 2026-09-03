# API Validación de Boletos - Documentación
## Equipo IA - MOA (Molinos Agro)

**Versión:** 1.0  
**Última actualización:** 24/07/2026  
**Ambiente:** Producción (PRD2)  
**Autor:** Equipo DataAgro  

---

## Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Seguridad y Autenticación](#seguridad-y-autenticación)
3. [Endpoints](#endpoints)
4. [Guía de Consumo para el Equipo IA](#guía-de-consumo-para-el-equipo-ia)
5. [Ejemplos de Uso](#ejemplos-de-uso)
6. [Estructura de Respuestas](#estructura-de-respuestas)
7. [Códigos de Error](#códigos-de-error)
8. [Notas Técnicas](#notas-técnicas)
9. [Arquitectura Interna](#arquitectura-interna-para-equipo-de-desarrollo)

---

## Descripción General

La API de **Validación de Boletos** proporciona acceso a información de contratos de compra de commodities (granos, maíz, etc.) y sus cláusulas asociadas. Esta API está diseñada para integración con sistemas de IA que requieren datos detallados sobre:

- **Datos de Contrato**: Material, precio, período de entrega, CUIT de partes, etc.
- **Cláusulas Aplicables**: Condiciones específicas según el tipo de boleto (Físico, Carta de Oferta, Confirma)

### Casos de Uso

- Validación automática de boletos mediante IA
- Extracción de datos contractuales para análisis
- Enriquecimiento de datos en sistemas de procesamiento de documentos
- Verificación de cláusulas obligatorias

---

## Seguridad y Autenticación

### Tipo de Autenticación: Token Bearer

Todos los requests a la API requieren un **token de autenticación Bearer** enviado en el header HTTP.

#### Header Requerido

```
Authorization: Bearer <TOKEN_API>
```

#### Token Actual (Producción PRD2)

```
DATA_AGRO_PROD_IA_2026
```

**Ubicación de la configuración:** `AWS.PRD2.DeployParameters.xml`
```xml
<setParameter name="ApiTokenDataAgro Key" value="DATA_AGRO_PROD_IA_2026" />
```

#### Validación de Seguridad

La validación del token se realiza en:
- **Componente:** `ApiRequestSecurityHelper` (ubicado en `WebDataAgro\Filters\`)
- **Mecanismo:** Token extraído desde:
  1. Variable de entorno: `ApiTokenEnvironmentVariable` (preferencia 1)
  2. Archivo de configuración `Web.config` - key: `ApiTokenDataAgro` (preferencia 2)

#### Rutas Protegidas

```
/api/validacionboletos/*
~/api/ValidacionBoletos/*
```

**Atributo de Filtro:** `[ApiTokenAuthorize]`

---

## Endpoints

### 1. GET /api/ValidacionBoletos/GetContrato

Obtiene los datos maestros de un contrato específico.

#### Parámetros

| Parámetro     | Tipo   | Ubicación | Requerido | Descripción                             |
|---------------|--------|-----------|-----------|-----------------------------------------|
| `contratoSAP` | string | Query     | Sí        | ID del contrato en SAP (ej: "SAP12345") |

#### Métodos Internos

- **Manager:** `ControlDeBoletosValidacionIAManagerIA.ObtenerDatosDeContrato()`
- **Consulta:** `TraerContratoBoletoPorSAP`
- **Repository:** `IRepositorio.ObtenerConsultaEscalar()`

#### Respuesta Exitosa (200 OK)

```json
{
  "material": "SOJA",
  "precio": 450.50,
  "destino": "PUERTO LA PLATA",
  "cosecha": "2024/2025",
  "tipoBoleto": "BOLETO DE COMPRA",
  "fechaOperacion": "15/02/2025",
  "periodoEntrega": "01/03/2025 - 31/05/2025",
  "cuitVendedor": "20-00000001-0",
  "cuitCorredor": "23-00000002-4",
  "moneda": "ARS",
  "provinciaOrigen": "Buenos Aires",
  "localidadOrigen": "Las Flores",
  "kilos": 50000,
  "fechaFijacionDesde": "15/02/2025",
  "fechaFijacionHasta": "15/03/2025",
  "bolsa": "BOLSA DE CEREALES DE BUENOS AIRES"
}
```

#### Respuesta - Contrato No Encontrado (404 Not Found)

```json
{
  "ok": false,
  "mensaje": "No se encontró el contrato."
}
```

#### Campos de Respuesta Detallados

| Campo                | Tipo    | Descripción                     | Ejemplo                             |
|----------------------|---------|---------------------------------|-------------------------------------|
| `material`           | string  | Tipo de commodity               | "SOJA", "MAIZ"                      |
| `precio`             | decimal | Precio por unidad               | 450.50                              |
| `destino`            | string  | Destino de la entrega           | "PUERTO LA PLATA"                   |
| `cosecha`            | string  | Campaña/Cosecha                 | "2024/2025"                         |
| `tipoBoleto`         | string  | Descripción del tipo de boleto  | "BOLETO DE COMPRA"                  |
| `fechaOperacion`     | string  | Fecha de operación (dd/MM/yyyy) | "15/02/2025"                        |
| `periodoEntrega`     | string  | Rango de fechas de entrega      | "01/03/2025 - 31/05/2025"           |
| `cuitVendedor`       | string  | CUIT del vendedor               | "20-00000001-0"                     |
| `cuitCorredor`       | string  | CUIT del corredor (si aplica)   | "23-00000002-4"                     |
| `moneda`             | string  | Moneda de transacción           | "ARS", "USD"                        |
| `provinciaOrigen`    | string  | Provincia de origen             | "Buenos Aires"                      |
| `localidadOrigen`    | string  | Localidad de origen             | "Las Flores"                        |
| `kilos`              | decimal | Cantidad en kilogramos          | 50000                               |
| `fechaFijacionDesde` | string  | Inicio del período de fijación  | "15/02/2025"                        |
| `fechaFijacionHasta` | string  | Fin del período de fijación     | "15/03/2025"                        |
| `bolsa`              | string  | Bolsa de cereales               | "BOLSA DE CEREALES DE BUENOS AIRES" |

---

### 2. GET /api/ValidacionBoletos/GetClausulas

Obtiene todas las cláusulas aplicables a un contrato según su tipo de boleto.

#### Parámetros

| Parámetro     | Tipo   | Ubicación | Requerido | Descripción            |
|---------------|--------|-----------|-----------|------------------------|
| `contratoSAP` | string | Query     | Sí        | ID del contrato en SAP |

#### Métodos Internos

- **Manager:** `ControlDeBoletosValidacionIAManagerIA.ObtenerClausulas()`
- **Consulta:** `TraerContratoBoletoPorSAP`
- **Repository:** `IRepositorio.ObtenerConsultaEscalar()`
- **Procesador:** `ProcesorClausulas<T>()` - Genérico

#### Lógica de Selección de Cláusulas

El endpoint determina automáticamente qué cláusulas aplicar según el **tipo de boleto**:

```
BoletoId = 1 (FISICO)
  → Servicio: IServicioClausulasBoletoFisico
  → Tabla: ClausulaBoletoFisico

BoletoId = 2 (CARTA_OFERTA)
  → Servicio: IServicioClausulasCartaOferta
  → Tabla: ClausulaCartaOferta

BoletoId = 3 (CONFIRMA)
  → Servicio: IServicioClausulasConfirma
  → Tabla: ClausulaConfirma
```

#### Respuesta Exitosa (200 OK)

```json
[
  {
    "orden": 1,
    "clausulaId": 101,
    "texto": "La entrega será en el Puerto de La Plata en la dirección indicada.",
    "aplicable": true,
    "categoria": "ENTREGA",
    "tipoClausula": "FISICO"
  },
  {
    "orden": 2,
    "clausulaId": 102,
    "texto": "El precio se fija conforme a los siguientes períodos...",
    "aplicable": true,
    "categoria": "FIJACION_DE_PRECIO",
    "tipoClausula": "FISICO"
  },
  {
    "orden": 3,
    "clausulaId": 103,
    "texto": "El pago se realizará contra entrega...",
    "aplicable": true,
    "categoria": "PAGO",
    "tipoClausula": "FISICO"
  }
]
```

#### Respuesta - Cláusulas No Encontradas (404 Not Found)

```json
{
  "ok": false,
  "mensaje": "No se encontraron las cláusulas."
}
```

#### Respuesta - Contrato No Existe (Vacío)

```json
[]
```

#### Campos de Respuesta Detallados

| Campo          | Tipo   | Descripción                                                      |
|----------------|--------|------------------------------------------------------------------|
| `orden`        | int    | Número de orden de la cláusula (1, 2, 3...)                      |
| `clausulaId`   | int    | ID único de la cláusula                                          |
| `texto`        | string | Texto completo de la cláusula                                    |
| `aplicable`    | bool   | Indica si la cláusula aplica al contrato                         |
| `categoria`    | string | Categoría temática (ENTREGA, FIJACION_DE_PRECIO, PAGO, etc.)     |
| `tipoClausula` | string | Tipo de boleto al que pertenece (FISICO, CARTA_OFERTA, CONFIRMA) |

---

## Guía de Consumo para el Equipo IA

### Requisitos Previos

1. **Token de API:** `DATA_AGRO_PROD_IA_2026`
2. **URL Base:** `https://DataAgro.com.ar/api/ValidacionBoletos`
3. **Protocolo:** HTTPS
4. **Content-Type:** application/json

### Flujo Recomendado

```
1. Recibir ID de contrato SAP
   ↓
2. Llamar a GetContrato para obtener datos maestros
   ↓
3. Validar que los datos sean consistentes (fechas, montos, etc.)
   ↓
4. Llamar a GetClausulas para obtener las condiciones aplicables
   ↓
5. Procesar y validar las cláusulas
   ↓
6. Generar resultado de validación
```

### Consideraciones de Consumo

#### Manejo de Valores Nulos

- Campos de fecha devuelven `string.Empty` si son nulos en BD
- Formato de fechas es siempre `dd/MM/yyyy`
- CUIT de corredor es vacío si `CorredorId <= 0`

#### Validación de Entrada

```python
# Pseudo-código
if not contratoSAP or contratoSAP.isspace():
    return []  # GetClausulas devuelve lista vacía
```

#### Rate Limiting

No hay limit explícito documentado. Se recomienda:
- Máximo 10 requests por segundo por token
- Implementar retry con backoff exponencial en caso de errores

#### Timeout Recomendado

- 30 segundos para GetContrato
- 45 segundos para GetClausulas (puede procesar múltiples cláusulas)

---

## Ejemplos de Uso

### Python

```python
import requests
import json

# Configuración
API_TOKEN = "DATA_AGRO_PROD_IA_2026"
BASE_URL = "https://DataAgro.com.ar/api/ValidacionBoletos"
HEADERS = {
    "Authorization": f"Bearer {API_TOKEN}",
    "Content-Type": "application/json"
}

# Función para obtener datos del contrato
def obtener_datos_contrato(contrato_sap):
    try:
        response = requests.get(
            f"{BASE_URL}/GetContrato",
            params={"contratoSAP": contrato_sap},
            headers=HEADERS,
            timeout=30
        )

        if response.status_code == 200:
            return response.json()
        elif response.status_code == 404:
            print(f"Contrato no encontrado: {contrato_sap}")
            return None
        else:
            print(f"Error: {response.status_code}")
            return None
    except requests.exceptions.RequestException as e:
        print(f"Error de conexión: {e}")
        return None

# Función para obtener cláusulas
def obtener_clausulas(contrato_sap):
    try:
        response = requests.get(
            f"{BASE_URL}/GetClausulas",
            params={"contratoSAP": contrato_sap},
            headers=HEADERS,
            timeout=45
        )

        if response.status_code == 200:
            return response.json()
        elif response.status_code == 404:
            print(f"Cláusulas no encontradas: {contrato_sap}")
            return []
        else:
            print(f"Error: {response.status_code}")
            return None
    except requests.exceptions.RequestException as e:
        print(f"Error de conexión: {e}")
        return None

# Uso
contrato_id = "SAP12345"

datos = obtener_datos_contrato(contrato_id)
if datos:
    print(f"Material: {datos['material']}")
    print(f"Precio: {datos['precio']}")
    print(f"Período de Entrega: {datos['periodoEntrega']}")

clausulas = obtener_clausulas(contrato_id)
if clausulas:
    for clausula in clausulas:
        print(f"[{clausula['orden']}] {clausula['texto'][:100]}...")
```

### JavaScript/Node.js

```javascript
const axios = require('axios');

const API_TOKEN = 'DATA_AGRO_PROD_IA_2026';
const BASE_URL = 'https://DataAgro.com.ar/api/ValidacionBoletos';

const client = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Authorization': `Bearer ${API_TOKEN}`,
    'Content-Type': 'application/json'
  },
  timeout: 30000
});

async function obtenerDatosContrato(contratoSAP) {
  try {
    const response = await client.get('/GetContrato', {
      params: { contratoSAP }
    });
    return response.data;
  } catch (error) {
    if (error.response?.status === 404) {
      console.log(`Contrato no encontrado: ${contratoSAP}`);
      return null;
    }
    console.error(`Error: ${error.message}`);
    return null;
  }
}

async function obtenerClausulas(contratoSAP) {
  try {
    const response = await client.get('/GetClausulas', {
      params: { contratoSAP }
    });
    return response.data;
  } catch (error) {
    if (error.response?.status === 404) {
      console.log(`Cláusulas no encontradas: ${contratoSAP}`);
      return [];
    }
    console.error(`Error: ${error.message}`);
    return null;
  }
}

// Uso
(async () => {
  const contratoId = 'SAP12345';

  const datos = await obtenerDatosContrato(contratoId);
  if (datos) {
    console.log(`Material: ${datos.material}`);
    console.log(`Precio: ${datos.precio}`);
  }

  const clausulas = await obtenerClausulas(contratoId);
  if (Array.isArray(clausulas)) {
    clausulas.forEach(c => {
      console.log(`[${c.orden}] ${c.texto.substring(0, 100)}...`);
    });
  }
})();
```

### cURL

```bash
#!/bin/bash

API_TOKEN="DATA_AGRO_PROD_IA_2026"
BASE_URL="https://DataAgro.com.ar/api/ValidacionBoletos"
CONTRATO_SAP="SAP12345"

# Obtener datos del contrato
echo "=== Datos del Contrato ==="
curl -X GET \
  "${BASE_URL}/GetContrato?contratoSAP=${CONTRATO_SAP}" \
  -H "Authorization: Bearer ${API_TOKEN}" \
  -H "Content-Type: application/json" \
  -w "\nHTTP Status: %{http_code}\n"

# Obtener cláusulas
echo -e "\n\n=== Cláusulas del Contrato ==="
curl -X GET \
  "${BASE_URL}/GetClausulas?contratoSAP=${CONTRATO_SAP}" \
  -H "Authorization: Bearer ${API_TOKEN}" \
  -H "Content-Type: application/json" \
  -w "\nHTTP Status: %{http_code}\n"
```

### C# / .NET

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class DataAgroApiClient
{
    private const string ApiToken = "DATA_AGRO_PROD_IA_2026";
    private const string BaseUrl = "https://DataAgro.com.ar/api/ValidacionBoletos";

    private readonly HttpClient _httpClient;

    public DataAgroApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiToken}");
    }

    public async Task<dynamic> ObtenerDatosContrato(string contratoSAP)
    {
        try
        {
            var url = $"{BaseUrl}/GetContrato?contratoSAP={contratoSAP}";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject(content);
            }

            Console.WriteLine($"Error {response.StatusCode}: {content}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            return null;
        }
    }

    public async Task<dynamic> ObtenerClausulas(string contratoSAP)
    {
        try
        {
            var url = $"{BaseUrl}/GetClausulas?contratoSAP={contratoSAP}";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject(content);
            }

            Console.WriteLine($"Error {response.StatusCode}: {content}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            return null;
        }
    }
}

// Uso
var client = new DataAgroApiClient();
var datos = await client.ObtenerDatosContrato("SAP12345");
var clausulas = await client.ObtenerClausulas("SAP12345");
```

---

## Estructura de Respuestas

### ValidacionDeBoletosIADatosContratoDto

DTOs internos (clase `Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA.ValidacionDeBoletosIADatosContratoDto`):

```csharp
public class ValidacionDeBoletosIADatosContratoDto
{
    public string Material { get; set; }
    public decimal? Precio { get; set; }
    public string Destino { get; set; }
    public string Cosecha { get; set; }
    public string TipoBoleto { get; set; }
    public string FechaOperacion { get; set; }
    public string PeriodoEntrega { get; set; }
    public string CuitVendedor { get; set; }
    public string CuitCorredor { get; set; }
    public string Moneda { get; set; }
    public string ProvinciaOrigen { get; set; }
    public string LocalidadOrigen { get; set; }
    public decimal? Kilos { get; set; }
    public string FechaFijacionDesde { get; set; }
    public string FechaFijacionHasta { get; set; }
    public string Bolsa { get; set; }
}
```

### ValidacionDeBoletosIAResultadoClausulaDto

```csharp
public class ValidacionDeBoletosIAResultadoClausulaDto
{
    public int Orden { get; set; }
    public int ClausulaId { get; set; }
    public string Texto { get; set; }
    public bool Aplicable { get; set; }
    public string Categoria { get; set; }
    public string TipoClausula { get; set; }
}
```

---

## Códigos de Error

| Código HTTP                   | Descripción            | Causa                                    | Acción Recomendada                              |
|-------------------------------|------------------------|------------------------------------------|-------------------------------------------------|
| **200 OK**                    | Éxito                  | La solicitud fue procesada correctamente | -                                               |
| **400 Bad Request**           | Solicitud inválida     | Parámetros mal formados                  | Verificar sintaxis de parámetros                |
| **401 Unauthorized**          | No autenticado         | Token faltante, inválido o expirado      | Verificar token en header Authorization         |
| **403 Forbidden**             | No autorizado          | Token válido pero sin permisos           | Contactar admins para permisos                  |
| **404 Not Found**             | Recurso no encontrado  | El contrato no existe en BD              | Verificar ID de contrato SAP                    |
| **500 Internal Server Error** | Error en servidor      | Fallo en lógica de negocio               | Reintentar; contactar equipo DevOps si persiste |
| **502 Bad Gateway**           | Gateway error          | Servidor no disponible                   | Reintentar después de unos segundos             |
| **503 Service Unavailable**   | Servicio no disponible | Mantenimiento programado                 | Reintentar después                              |

### Manejo de Errores Recomendado

```python
# Pseudocódigo
def llamar_api(endpoint, params):
    max_intentos = 3
    for intento in range(max_intentos):
        try:
            response = requests.get(endpoint, params=params, headers=headers, timeout=30)

            if response.status_code == 200:
                return response.json()
            elif response.status_code == 404:
                return None  # Contrato no existe
            elif response.status_code in [401, 403]:
                # Error de autenticación - no reintentar
                log_error(f"Auth error: {response.status_code}")
                return None
            elif response.status_code >= 500:
                # Error de servidor - reintentar
                esperar(2 ** intento)  # Backoff exponencial
                continue
            else:
                return None
        except requests.Timeout:
            esperar(2 ** intento)
            continue
        except Exception as e:
            log_error(f"Error: {e}")
            return None

    return None  # Falló después de reintentos
```

---

## Notas Técnicas

### Performance

- **GetContrato**: ~100-200ms (consulta simple con escalares)
- **GetClausulas**: ~200-500ms (múltiples cláusulas según tipo de boleto)
- **Recomendación**: Implementar caché en cliente cuando sea posible

### Limitaciones Conocidas

1. **PeriodoEntrega**: Tiene un bug de concatenación de strings (falta paréntesis)
   ```csharp
   // Código actual (problema):
   datosContrato.PeriodoEntrega = contrato.FechaDesde != null ? 
     contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : string.Empty + " - " + 
     contrato.FechaHasta != null ? 
     contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : string.Empty;

   // Resultado: puede devolver "FechaDesde - " si FechaHasta es null
   // Recomendación: Validar siempre ambas fechas
   ```

2. **TipoBoleto se asigna dos veces**: La línea que asigna `TipoBoleto = contrato.BoletoDescripcion` aparece dos veces (línea 54 y 64 en el código fuente)

3. **Sin paginación**: GetClausulas devuelve TODAS las cláusulas; no hay límite ni offset

### Validación de Datos

Campos que pueden ser nulos o vacíos:
- `FechaOperacion` → devuelve `string.Empty`
- `PeriodoEntrega` → parcialmente (ver limitaciones)
- `FechaFijacionDesde`, `FechaFijacionHasta` → devuelven `string.Empty`
- `CuitCorredor` → devuelve `string.Empty` si `CorredorId <= 0`

### Thread Safety

El manager es inyectado por DI (Dependency Injection). Es seguro usar en modo concurrente.

### Seguridad Adicional

- Token no debe exponerse en logs o responses
- Usar HTTPS siempre (no HTTP)
- Token debe estar en variables de entorno, no hardcodeado
- Implementar rotación periódica de tokens

---

## Arquitectura Interna (Para Equipo de Desarrollo)

### Componentes Principales

```
                    ┌─────────────────────┐
                    │  ValidacionBoletos  │
                    │    Controller       │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │ IControlDeBoletos   │
                    │ ValidacionIA        │
                    │ Manager (Interface) │
                    └──────────┬──────────┘
                               │
┌──────────────────────────────▼───────────────────────────────┐
│  ControlDeBoletosValidacionIAManagerIA (Implementación)      │
└───────────────────────────┬─────────────────────────────────-┘
                            │
            ┌───────────────┼───────────────┐
            │               │               │
    ┌───────▼────┐  ┌───────▼──────┐  ┌────▼──────---┐
    │ IRepositorio  │ ILogger      │  │ IMailManager │
    │            │  │              │  │              │
    └────────────┘  └────────------┘  └──────────---─┘
            │
    ┌───────▼────────────────----┐
    │  ConsultasEF               │
    │ (TraerContratoBoletoPorSAP)│
    └────────────────────────----┘
```

### Flujo de ObtenerDatosDeContrato

```
1. Recibe: string contratoSAP
   ↓
2. Crea instancia: ValidacionDeBoletosIADatosContratoDto
   ↓
3. Ejecuta consulta:
   repositorio.ObtenerConsultaEscalar(
     new TraerContratoBoletoPorSAP(contratoSAP)
   )
   ↓
4. Si contrato null → devuelve DTO vacío
   ↓
5. Si contrato ≠ null → mapea propiedades:
   - Material, Precio, Destino, etc.
   - Convierte fechas a formato dd/MM/yyyy
   - Valida nullability
   ↓
6. Devuelve DTO completado
```

### Flujo de ObtenerClausulas

```
1. Recibe: string contratoSAP
   ↓
2. Valida: if (string.IsNullOrWhiteSpace(contratoSAP))
   → Devuelve lista vacía
   ↓
3. Ejecuta consulta master (una sola):
   repositorio.ObtenerConsultaEscalar(
     new TraerContratoBoletoPorSAP(contratoSAP)
   )
   ↓
4. Si basicoContrato == null:
   → Log warn y devuelve lista vacía
   ↓
5. Switch basado en basicoContrato.BoletoId:
   │
   ├─ FISICO (1) → ObtenerClausulasBoletoFisico()
   │               → repositorio.Listar<ClausulaBoletoFisico>()
   │               → servicioClausulasBoletoFisico.DevolverClausulas()
   │
   ├─ CARTA_OFERTA (2) → ObtenerClausulasCartaOferta()
   │                     → repositorio.Listar<ClausulaCartaOferta>()
   │                     → servicioClausulasCartaOferta.DevolverClausulas()
   │
   └─ CONFIRMA (3) → ObtenerClausulasConfirma()
                      → repositorio.Listar<ClausulaConfirma>()
                      → servicioClausulaConfirma.DevolverClausulas()
   ↓
6. Procesa con ProcesorClausulas<T>():
   - Inicializa datos del contrato en cada cláusula
   - Ejecuta servicio correspondiente
   - Construye resultado con orden, texto, etc.
   ↓
7. Devuelve List<ValidacionDeBoletosIAResultadoClausulaDto>
```

### Inyección de Dependencias

```csharp
// Registro en contenedor IoC (presumido)
container.RegisterSingleton<ILogger>();
container.RegisterSingleton<IRepositorio>();
container.RegisterSingleton<IMailManager>();
container.RegisterSingleton<IServicioClausulasCartaOferta>();
container.RegisterSingleton<IServicioClausulasBoletoFisico>();
container.RegisterSingleton<IServicioClausulasGenericos>();
container.RegisterSingleton<IServicioClausulasConfirma>();
container.RegisterSingleton<IControlDeBoletosValidacionIAManagerIA, 
  ControlDeBoletosValidacionIAManagerIA>();
```

### Métodos Privados Clave

#### ObtenerClausulasConfirma(BasicoContrato basico)

- Extrae todas las instancias de `ClausulaConfirma` del repositorio
- Delega al procesador genérico con servicio `servicioClausulaConfirma`

#### ObtenerClausulasCartaOferta(BasicoContrato basico)

- Extrae todas las instancias de `ClausulaCartaOferta` del repositorio
- Delega al procesador genérico con servicio `servicioClausulasCartaOferta`

#### ObtenerClausulasBoletoFisico(BasicoContrato basico)

- Extrae todas las instancias de `ClausulaBoletoFisico` del repositorio
- Delega al procesador genérico con servicio `servicioClausulasBoletoFisico`

#### ProcesorClausulas<T>()

Procesador genérico que:

1. **Inicializa**: Ejecuta `Inicializar(basico)` (método no mostrado)
2. **Itera**: Recorre cada cláusula
3. **Asigna Basico**: `clausulaConBasico.Basico = basico` (asignación dinámica)
4. **Obtiene Resultado**: Ejecuta `devolverClausula(item)`
5. **Filtra**: Solo agrega si texto no es nulo/vacío
6. **Enumera**: Asigna orden secuencial (1, 2, 3...)
7. **Construye DTO**: `ValidacionDeBoletosIAResultadoClausulaDto`

### Consultas de Base de Datos

#### TraerContratoBoletoPorSAP

- **Tabla Principal**: `BasicoContrato` (presumida)
- **Campos Retornados**:
  - Material, Precio, DestinoDescripcion, Campania, BoletoDescripcion
  - FechaOperacion, FechaDesde, FechaHasta
  - Cuit, CorredorId, CUITCorredor
  - Moneda, Provincia, Localidad, Cantidad
  - DesdeFijacion, HastaFijacion
  - BolsaDescripcion, BoletoId
- **Parámetro**: SAP Contract Number (string)

### Enumeraciones Clave

#### EnumBoletoCompraNet

```csharp
public enum EnumBoletoCompraNet
{
    FISICO = 1,
    CARTA_OFERTA = 2,
    CONFIRMA = 3
}
```

### Logs

El manager registra:
- **Nivel WARN**: Cuando un contrato no es encontrado en `ObtenerClausulas()`
- **No registra**: Datos sensibles de contratos (CUIT, precios, etc.)

---

## Tabla Resumen de Endpoints

| Endpoint        | Método | Parámetros    | Retorna                  | Caso Uso               |
|-----------------|--------|---------------|--------------------------|------------------------|
| `/GetContrato`  | GET    | `contratoSAP` | `DTO` de contrato        | Obtener datos maestros |
| `/GetClausulas` | GET    | `contratoSAP` | `List<DTO>` de cláusulas | Obtener condiciones    |

---

## Contacto y Soporte

**Equipo DataAgro**
- Repositorio: `https://dev.azure.com/molinosagro/DataAgro/_git/DataAgro`
- Rama actual: `qa`
- Ambiente: Producción PRD2

**Información de Configuración**
- Token gestión: `AWS.PRD2.DeployParameters.xml`
- Implementación filtros: `WebDataAgro\Filters\ApiRequestSecurityHelper.cs`
- Controlador: `WebDataAgro\Controllers\ValidacionBoletosDAController.cs`

---

## Versionado

| Versión | Fecha       | Cambios               |
|---------|-------------|-----------------------|
| 1.0     | 24/07/2026  | Documentación inicial |

---

**Fecha de generación:** 24/07/2026  
**Próxima revisión:** 
**Clasificación:** Interno / API para Equipo IA MOA
