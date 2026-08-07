CREATE PROCEDURE dbo.DataAgro_BusquedaValidacionBoletosPendientes
    @NegocioSAP             NVARCHAR(MAX) = NULL,  -- contratos SAP separados por ';'
    @MaterialId             INT           = NULL,
    @EstadoValidacionId     INT           = NULL,
    @FechaValidacionDesde   DATETIME      = NULL,
    @FechaValidacionHasta   DATETIME      = NULL,
    @ProveedorId            INT           = NULL,
    @BolsaId                INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    -- Tabla temporal: contratos SAP buscados
    
    CREATE TABLE #NegocioSAP (ContratoSAP NVARCHAR(50));
    IF @NegocioSAP IS NOT NULL AND LEN(LTRIM(RTRIM(@NegocioSAP))) > 0
        INSERT INTO #NegocioSAP (ContratoSAP)
        SELECT RIGHT('0000000000' + LTRIM(RTRIM(value)), 10)
        FROM STRING_SPLIT(@NegocioSAP, ';')
        WHERE LTRIM(RTRIM(value)) <> '';

    SELECT
        val.Id,
        bc.Descripcion                                                           AS TipoBoleto,
        n.ContratoSAP,
        (case when bc.Id = 1 then confirma.Version else boleto.Version end)                 AS Version,
        (case when bc.Id = 1 then confirma.FechaGeneracion else boleto.FechaGeneracion end) AS FechaGeneracion , 
        bolsa.Descripcion                                                        AS BolsaCompraNet,
        mat.Descripcion                                                          AS Material,        
        prov.RazonSocial                                                         AS Proveedor,
        est.Descripcion                                                          AS ValidacionBoletosEstado,
        est.Id                                                                   AS ValidacionBoletosEstadoId,
        val.FechaCreacion                                                        AS FechaValidacion,
        val.FechaRechazo                                                         AS FechaRechazo,
        val.RequestId                                                            AS RequestId,
        ISNULL(val.EstadoValidacionAgente, '')                                   AS EstadoValidacionAgente,
        ISNULL(val.AccionesRecomendadas, '')                                     AS AccionesRecomendadas,
        ISNULL(val.Observacion, '')                                              AS Observacion
    FROM ControlDeBoletos cb
    INNER JOIN Negocio n
        ON cb.NegocioId = n.Id
    INNER JOIN ValidacionBoletos val 
        ON val.ControlDeBoletosId = cb.Id
    LEFT JOIN ValidacionBoletosEstado est
        ON est.Id = val.ValidacionBoletosEstadoId
    -- TipoBoleto viene del negocio (para fijaciones usa el boleto del contrato padre)
    LEFT JOIN Negocio nPadre
        ON n.TipoNegocioId = 3 AND n.ContratoId = nPadre.Id
    LEFT JOIN BoletoCompraNet bc
        ON bc.Id = CASE WHEN n.TipoNegocioId = 3 THEN nPadre.BoletoId ELSE n.BoletoId END
    LEFT JOIN Material mat
        ON n.MaterialId = mat.MaterialId
    LEFT JOIN BolsaCompraNet bolsa
        ON n.BolsaId = bolsa.Id
    LEFT JOIN Proveedor prov
        ON n.ProveedorId = prov.ProveedorId
    OUTER APPLY (
        SELECT TOP 1 s.Version, s.FechaGeneracion
        FROM Boleto s
        WHERE s.NegocioId = cb.NegocioId
        ORDER BY s.FechaGeneracion DESC
    ) boleto

    OUTER APPLY (
        SELECT TOP 1 s.Version, s.FechaGeneracion
        FROM Confirma s
        WHERE s.NegocioId = cb.NegocioId
        ORDER BY s.FechaGeneracion DESC
    ) confirma 
    WHERE
        -- Filtro base de estado
        (@EstadoValidacionId IS NULL OR val.ValidacionBoletosEstadoId = @EstadoValidacionId)

        -- Bloque NegocioSAP (tiene prioridad sobre el resto de filtros, igual que en EF)
        AND (
            @NegocioSAP IS NULL
            OR n.ContratoSAP IN (SELECT ContratoSAP FROM #NegocioSAP)
        )

        -- Filtros opcionales (solo cuando NO se busca por SAP)
        AND (@NegocioSAP IS NOT NULL OR @MaterialId      IS NULL OR n.MaterialId      = @MaterialId)
        AND (@NegocioSAP IS NOT NULL OR @FechaValidacionDesde IS NULL OR val.FechaCreacion >= @FechaValidacionDesde)
        AND (@NegocioSAP IS NOT NULL OR @FechaValidacionHasta IS NULL OR val.FechaCreacion  < DATEADD(DAY, 1, CAST(@FechaValidacionHasta AS DATE)))
        AND (@NegocioSAP IS NOT NULL OR @ProveedorId     IS NULL OR n.ProveedorId     = @ProveedorId)
        AND (@NegocioSAP IS NOT NULL OR @BolsaId         IS NULL OR n.BolsaId         = @BolsaId)
END
