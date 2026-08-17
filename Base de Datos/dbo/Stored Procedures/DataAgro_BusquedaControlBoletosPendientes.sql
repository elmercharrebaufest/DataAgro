CREATE PROCEDURE dbo.DataAgro_BusquedaControlBoletosPendientes
    @NegocioSAP      NVARCHAR(MAX) = NULL,  -- contratos SAP separados por ';'
    @MaterialId      INT           = NULL,
    @EstadoControlId INT           = NULL,
    @EsConfirma      BIT           = NULL,
    @FechaCargaDesde DATETIME      = NULL,
    @FechaCargaHasta DATETIME      = NULL,
    @ProveedorId     INT           = NULL,
    @BolsaId         INT           = NULL,
    @ComercialId     INT           = NULL
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
        cb.Id,
        cb.NegocioId,
        cb.ControlDeBoletosEstadoId,
        cbe.Descripcion                                                          AS ControlDeBoletosEstado,
        cb.EsConfirma,
        cb.AltaIdLoteConfirma,
        cb.IdentificadorConfirma,
        cb.FechaCreacion,
        cb.FechaModificacion,
        cb.EstadoConfirmaId,
        ec.Descripcion                                                           AS EstadoConfirma,
        bc.Descripcion                                                           AS TipoBoleto,
        cb.ControlIniciado,
        cb.ControlFinalizado,
        cb.CertificacionCompletada,
        cb.RegistroDatosOblea,
        cb.FechaControlIniciado,
        cb.FechaControlFinalizado,
        cb.FechaCertificacionCompletada,
        cb.FechaRegistroDatosOblea,
        n.MaterialId,
        mat.Descripcion                                                          AS Material,
        n.BolsaId                                                                AS BolsaCompraNetId,
        bolsa.Descripcion                                                        AS BolsaCompraNet,
        n.ComercialId,
        CASE
            WHEN com.ComercialId IS NOT NULL
            THEN com.Nombres + ' ' + com.Apellido
            ELSE NULL
        END                                                                      AS Comercial,
        n.ContratoSAP,
        n.ProveedorId,
        prov.RazonSocial                                                         AS Proveedor,
        seg.Id                                                                   AS SeguimientoBoletoId,
        CASE
            WHEN cb.EsConfirma = 1 AND cb.EsConfirmaAltaBorrador = 1 AND cb.AltaIdLoteConfirma IS NOT NULL THEN 'Alta Borrador'
            WHEN cb.EsConfirma = 1 AND cb.EsConfirmaAltaBorrador = 0 AND cb.AltaIdDocumentoConfirma IS NOT NULL THEN 'Alta Definitiva'
            ELSE ''
        END                                                                      AS TipoAltaConfirma,
        (case when bc.Id = 1 then confirma.Version else boleto.Version end)                 AS Version,
        (case when bc.Id = 1 then confirma.FechaGeneracion else boleto.FechaGeneracion end) AS FechaGeneracion,
        n.BoletoId                                                               AS TipoBoletoId
    FROM ControlDeBoletos cb

    INNER JOIN Negocio n
        ON cb.NegocioId = n.Id

    LEFT JOIN ControlDeBoletosEstado cbe
        ON cb.ControlDeBoletosEstadoId = cbe.Id

    LEFT JOIN EstadoConfirma ec
        ON cb.EstadoConfirmaId = ec.Id

    -- TipoBoleto viene del negocio (para fijaciones usa el boleto del contrato padre)
    LEFT JOIN Negocio nPadre
        ON n.TipoNegocioId = 3 AND n.ContratoId = nPadre.Id

    LEFT JOIN BoletoCompraNet bc
        ON bc.Id = CASE WHEN n.TipoNegocioId = 3 THEN nPadre.BoletoId ELSE n.BoletoId END

    LEFT JOIN Material mat
        ON n.MaterialId = mat.MaterialId

    LEFT JOIN BolsaCompraNet bolsa
        ON n.BolsaId = bolsa.Id

    LEFT JOIN Comercial com
        ON n.ComercialId = com.ComercialId

    LEFT JOIN Proveedor prov
        ON n.ProveedorId = prov.ProveedorId

    -- Solo el último registro de seguimiento por ControlDeBoletos
    OUTER APPLY (
        SELECT TOP 1 s.Id
        FROM ControlDeBoletosSeguimiento s
        WHERE s.ControlDeBoletosId = cb.Id
        ORDER BY s.Id DESC
    ) seg
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
        (@EstadoControlId IS NULL OR cb.ControlDeBoletosEstadoId = @EstadoControlId)

        -- Bloque NegocioSAP (tiene prioridad sobre el resto de filtros, igual que en EF)
        AND (
            @NegocioSAP IS NULL
            OR n.ContratoSAP IN (SELECT ContratoSAP FROM #NegocioSAP)
        )

        -- Filtros opcionales (solo cuando NO se busca por SAP)
        AND (@NegocioSAP IS NOT NULL OR @MaterialId      IS NULL OR n.MaterialId      = @MaterialId)
        AND (@NegocioSAP IS NOT NULL OR @EsConfirma      IS NULL OR cb.EsConfirma     = @EsConfirma)
        AND (@NegocioSAP IS NOT NULL OR @FechaCargaDesde IS NULL OR cb.FechaCreacion >= @FechaCargaDesde)
        AND (@NegocioSAP IS NOT NULL OR @FechaCargaHasta IS NULL OR cb.FechaCreacion  < DATEADD(DAY, 1, CAST(@FechaCargaHasta AS DATE)))
        AND (@NegocioSAP IS NOT NULL OR @ProveedorId     IS NULL OR n.ProveedorId     = @ProveedorId)
        AND (@NegocioSAP IS NOT NULL OR @BolsaId         IS NULL OR n.BolsaId         = @BolsaId)
        AND (@NegocioSAP IS NOT NULL OR @ComercialId     IS NULL OR n.ComercialId     = @ComercialId);
END
