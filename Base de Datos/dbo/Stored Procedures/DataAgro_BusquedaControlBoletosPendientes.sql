CREATE PROCEDURE [dbo].[DataAgro_BusquedaControlBoletosPendientes]
    @NegocioSAP      NVARCHAR(MAX) = NULL,  -- contratos SAP separados por ';'
    @MaterialId      INT           = NULL,
    @EstadoControlId INT           = NULL,
    @EsConfirma      BIT           = NULL,
    @EsBoletoFisico  BIT           = NULL,
    @EsCartaOferta   BIT           = NULL,
    @EsSinBoleto     BIT           = NULL,
    @EsNinguno       BIT           = NULL,
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
    DECLARE @tbl_NegocioSAP Table (ContratoSAP NVARCHAR(50));

    IF @NegocioSAP IS NOT NULL AND LEN(LTRIM(RTRIM(@NegocioSAP))) > 0
        INSERT INTO @tbl_NegocioSAP (ContratoSAP)
        SELECT RIGHT('0000000000' + LTRIM(RTRIM(value)), 10)
        FROM STRING_SPLIT(@NegocioSAP, ';')
        WHERE LTRIM(RTRIM(value)) <> '';
    
    DECLARE @tbl_TipoBoletos Table (BoletoId int)


    IF EXISTS (SELECT 1 FROM @tbl_NegocioSAP)
       BEGIN
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(1)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(2)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(4)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(5)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(3)
       END
       ELSE
       BEGIN
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(CASE WHEN @EsConfirma = 1 THEN 1 ELSE 0 END)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(CASE WHEN @EsBoletoFisico = 1 THEN 2 ELSE 0 END)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(CASE WHEN @EsCartaOferta = 1 THEN 4 ELSE 0 END)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(CASE WHEN @EsSinBoleto = 1 THEN 5 ELSE 0 END)
            INSERT INTO @tbl_TipoBoletos (BoletoId)VALUES(CASE WHEN @EsNinguno = 1 THEN 3 ELSE 0 END)
            DELETE FROM @tbl_TipoBoletos WHERE BoletoId = 0
       END

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
            OR n.ContratoSAP IN (SELECT ContratoSAP FROM @tbl_NegocioSAP)
        )

        -- Filtros opcionales (solo cuando NO se busca por SAP)
        AND (@NegocioSAP IS NOT NULL OR @MaterialId      IS NULL OR n.MaterialId      = @MaterialId)
        AND (n.BoletoId IN (SELECT BoletoId FROM @tbl_TipoBoletos))

        AND (@NegocioSAP IS NOT NULL OR @FechaCargaDesde IS NULL OR cb.FechaCreacion >= @FechaCargaDesde)
        AND (@NegocioSAP IS NOT NULL OR @FechaCargaHasta IS NULL OR cb.FechaCreacion  < DATEADD(DAY, 1, CAST(@FechaCargaHasta AS DATE)))
        AND (@NegocioSAP IS NOT NULL OR @ProveedorId     IS NULL OR n.ProveedorId     = @ProveedorId)
        AND (@NegocioSAP IS NOT NULL OR @BolsaId         IS NULL OR n.BolsaId         = @BolsaId)
        AND (@NegocioSAP IS NOT NULL OR @ComercialId     IS NULL OR n.ComercialId     = @ComercialId);
END
