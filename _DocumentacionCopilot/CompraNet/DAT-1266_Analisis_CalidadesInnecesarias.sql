/* ================================================================
   DAT-1266 - Evaluar posibilidad de sacar Calidades innecesarias
   ================================================================
   Combinaciones bajo análisis:
     1) Material TRIGO   + Standard de Calidad "Especial"
        (Negocio.StandardDeCalidadId directo, o registrado como
         detalle en la tabla Calidad)
     2) Material GIRASOL + Calidad Especial "Materia Extraña"
        (registrado como detalle en la tabla Calidad -> CalidadEspecial)

   Supuesto: se usa Negocio.Fecha como "fecha de carga" del negocio.
   Si lo que se necesita es la fecha real de alta en el sistema,
   reemplazar Fecha por FechaOperacion en el bloque de #NegociosObjetivo.

   Ejecutar el script completo de una sola vez (usa una tabla temporal
   #NegociosObjetivo que alimenta las 5 consultas de análisis).
   ================================================================ */

SET NOCOUNT ON;

DECLARE @FechaDesde DATETIME = DATEADD(DAY, -365, CAST(GETDATE() AS DATE));

IF OBJECT_ID('tempdb..#NegociosObjetivo') IS NOT NULL DROP TABLE #NegociosObjetivo;

-- Universo de negocios que cumplen alguna de las dos combinaciones bajo análisis
SELECT
    n.Id            AS NegocioId,
    n.Fecha         AS Fecha,
    n.UsuarioId     AS Usuario,
    m.Descripcion   AS Material,
    ec.Descripcion  AS Estado,
    'TRIGO + Especial' AS Combinacion
INTO #NegociosObjetivo
FROM Negocio n
INNER JOIN Material m ON m.MaterialId = n.MaterialId
INNER JOIN EstadoContrato ec ON ec.EstadoContratoId = n.EstadoId
INNER JOIN StandardDeCalidad scDirecta ON scDirecta.Id = n.StandardDeCalidadId
WHERE m.Descripcion LIKE 'TRIGO%'
  AND scDirecta.Descripcion = 'Especial'

UNION

SELECT
    n.Id,
    n.Fecha,
    n.UsuarioId,
    m.Descripcion,
    ec.Descripcion,
    'TRIGO + Especial'
FROM Negocio n
INNER JOIN Material m ON m.MaterialId = n.MaterialId
INNER JOIN EstadoContrato ec ON ec.EstadoContratoId = n.EstadoId
WHERE m.Descripcion LIKE 'TRIGO%'
  AND EXISTS (
        SELECT 1
        FROM Calidad c
        INNER JOIN StandardDeCalidad scDetalle ON scDetalle.Id = c.StandardDeCalidadId
        WHERE c.NegocioId = n.Id AND scDetalle.Descripcion = 'Especial'
  )

UNION

SELECT
    n.Id,
    n.Fecha,
    n.UsuarioId,
    m.Descripcion,
    ec.Descripcion,
    'GIRASOL + Materia Extraña'
FROM Negocio n
INNER JOIN Material m ON m.MaterialId = n.MaterialId
INNER JOIN EstadoContrato ec ON ec.EstadoContratoId = n.EstadoId
WHERE m.Descripcion LIKE 'GIRASOL%'
  AND EXISTS (
        SELECT 1
        FROM Calidad c
        INNER JOIN CalidadEspecial ce ON ce.Id = c.CalidadEspecialId
        WHERE c.NegocioId = n.Id AND ce.Descripcion = 'Materia Extraña'
  );

CREATE INDEX IX_NegObj_Combinacion ON #NegociosObjetivo (Combinacion, Fecha);


/* ----------------------------------------------------------------
   1) Resumen general: total histórico y fecha de última/primera carga
   ---------------------------------------------------------------- */
SELECT
    Combinacion,
    COUNT(*)   AS TotalRegistrosHistorico,
    MAX(Fecha) AS FechaUltimaCarga,
    MIN(Fecha) AS FechaPrimeraCarga
FROM #NegociosObjetivo
GROUP BY Combinacion
ORDER BY Combinacion;


/* ----------------------------------------------------------------
   2) Cantidad de registros cargados en los últimos 365 días
   ---------------------------------------------------------------- */
SELECT
    Combinacion,
    COUNT(*) AS RegistrosUltimos365Dias
FROM #NegociosObjetivo
WHERE Fecha >= @FechaDesde
GROUP BY Combinacion
ORDER BY Combinacion;


/* ----------------------------------------------------------------
   3) Distribución mensual de cargas (últimos 365 días)
   ---------------------------------------------------------------- */
SELECT
    Combinacion,
    CONVERT(CHAR(7), Fecha, 120) AS AnioMes, -- yyyy-MM
    COUNT(*)                     AS CantidadRegistros
FROM #NegociosObjetivo
WHERE Fecha >= @FechaDesde
GROUP BY Combinacion, CONVERT(CHAR(7), Fecha, 120)
ORDER BY Combinacion, AnioMes;


/* ----------------------------------------------------------------
   4) Usuarios que realizaron las cargas y cantidad por usuario
      (últimos 365 días)
   ---------------------------------------------------------------- */
SELECT
    Combinacion,
    ISNULL(Usuario, '(Sin usuario)') AS Usuario,
    COUNT(*)                         AS CantidadRegistros
FROM #NegociosObjetivo
WHERE Fecha >= @FechaDesde
GROUP BY Combinacion, Usuario
ORDER BY Combinacion, CantidadRegistros DESC;


/* ----------------------------------------------------------------
   4b) Usuarios y cantidad - histórico completo (sin filtro de fecha)
       Útil para comparar con el punto anterior
   ---------------------------------------------------------------- */
SELECT
    Combinacion,
    ISNULL(Usuario, '(Sin usuario)') AS Usuario,
    COUNT(*)                         AS CantidadRegistros
FROM #NegociosObjetivo
GROUP BY Combinacion, Usuario
ORDER BY Combinacion, CantidadRegistros DESC;


/* ----------------------------------------------------------------
   5) Detalle de negocios (para inspección puntual / exportar)
   ---------------------------------------------------------------- */
SELECT *
FROM #NegociosObjetivo
ORDER BY Combinacion, Fecha DESC;


DROP TABLE #NegociosObjetivo;
