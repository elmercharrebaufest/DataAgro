CREATE PROCEDURE dbo.DataAgro_BusquedaBoletoFisico
    @NegocioSAP             NVARCHAR(MAX) = NULL,   -- SAP separados por ';'
    @MaterialId             INT           = NULL,
    @ProveedorId            INT           = NULL,
    @ComercialId            INT           = NULL,
    @BolsaCompraNetId       INT           = NULL,
    @FechaConfirmacionDesde DATETIME      = NULL,
    @FechaConfirmacionHasta DATETIME      = NULL,
    @Equipo                 NVARCHAR(MAX) = NULL    -- IDs de comercial separados por ','
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    -- Tabla temporal: IDs de equipo
    CREATE TABLE #Equipo (ComercialId INT);
    IF @Equipo IS NOT NULL AND LEN(@Equipo) > 0
        INSERT INTO #Equipo (ComercialId)
        SELECT CAST(LTRIM(RTRIM(value)) AS INT)
        FROM STRING_SPLIT(@Equipo, ',')
        WHERE LTRIM(RTRIM(value)) <> '';

    -- Tabla temporal: contratos SAP buscados
    CREATE TABLE #NegocioSAP (ContratoSAP NVARCHAR(50));
    IF @NegocioSAP IS NOT NULL AND LEN(@NegocioSAP) > 0
        INSERT INTO #NegocioSAP (ContratoSAP)
        SELECT LTRIM(RTRIM(value))
        FROM STRING_SPLIT(@NegocioSAP, ';')
        WHERE LTRIM(RTRIM(value)) <> '';

    -- Último boleto vigente por negocio
    ;WITH UltimoBoleto AS (
        SELECT
            b.NegocioId,
            MAX(b.Version) AS MaxVersion
        FROM Boleto b
        WHERE b.FechaAnulacion IS NULL
        GROUP BY b.NegocioId
    ),
    BoletoDetalle AS (
        SELECT b.NegocioId, b.Version, b.FechaGeneracion, b.FechaAnulacion
        FROM Boleto b
        INNER JOIN UltimoBoleto ub
            ON b.NegocioId = ub.NegocioId AND b.Version = ub.MaxVersion
        WHERE b.FechaAnulacion IS NULL
    )
    SELECT
        n.Id,

        -- NegocioSAP: para FIJACION usa FijacionSAP
        CASE WHEN n.TipoNegocioId = 3 THEN n.FijacionSAP ELSE n.ContratoSAP END AS NegocioSAP,
        n.ContratoSAP,
        CASE WHEN n.TipoNegocioId = 3 THEN n.FijacionSAP ELSE '' END            AS FijacionSAP,

        -- ClaseNegocioId: 1 = Contrato, 2 = FijacionDePrecioContrato
        CASE WHEN n.Discriminator = 'Contrato' THEN '1' ELSE '2' END            AS ClaseNegocioId,
        n.TipoNegocioId,

        -- TipoNegocio: replica la lógica de is/as en EF (orden importante)
        CASE
            WHEN n.Discriminator = 'Contrato'             AND n.Madre              = 1 THEN 'CONVENIO'
            WHEN n.Discriminator = 'Contrato'             AND n.Madre              = 0 THEN 'FIJ. CONVENIO'
            WHEN n.Discriminator = 'Contrato'             AND n.EsFason            = 1 THEN 'FASON MP'
            WHEN n.Discriminator = 'Contrato'             AND n.TipoAgenteCompraId > 0 THEN 'AGENTE DE COMPRAS MP'
            WHEN n.Discriminator = 'ContratoAcuerdo'      AND n.TipoAgenteCompraId > 0 THEN 'ACUERDO AGENTE'
            WHEN n.Discriminator = 'Contrato'             AND n.Canje              = 1 THEN 'CANJE'
            WHEN n.Discriminator = 'Contrato'             AND n.PrestamoDevolucion = 1 THEN 'PRESTAMO DEVOLUCION'
            WHEN n.Discriminator = 'Contrato'             AND n.Venta              = 1 THEN 'VENTA'
            WHEN n.Discriminator = 'Contrato'             AND n.TipoPosicionCBOTId = 3 THEN 'A FIJAR PASE'
            WHEN n.Discriminator = 'FijacionDePrecioContrato' AND n.Virtual        = 1 THEN 'FIJACION VIRTUAL'
            WHEN n.Discriminator = 'FijacionDePrecioContrato' AND n.Canje          = 1 THEN 'FIJACION CANJE'
            WHEN n.Discriminator = 'FijacionDePrecioContrato' AND n.TipoPosicionCBOTId = 3 THEN 'FIJACION PASE'
            ELSE tn.Descripcion
        END AS TipoNegocio,

        -- Versión del boleto
        CASE WHEN bd.Version > 1 THEN bd.Version ELSE 1 END AS Version,

        -- Estado del boleto
        CASE
            WHEN bd.FechaAnulacion  IS NOT NULL THEN 'Anulado'
            WHEN bd.FechaGeneracion IS NOT NULL THEN 'Vigente'
            ELSE 'Pendiente'
        END AS Estado_Version,

        -- BoletoId / TipoBoleto: para FIJACION viene del contrato padre
        CASE WHEN n.TipoNegocioId = 3 THEN nPadre.BoletoId    ELSE n.BoletoId          END AS BoletoId,
        CASE WHEN n.TipoNegocioId = 3 THEN bcPadre.Descripcion ELSE bc.Descripcion     END AS TipoBoleto,

        -- Canje
        CASE WHEN n.Canje = 1 THEN 'SI' ELSE 'NO' END AS Canje,

        -- Bolsa: para FIJACION viene del contrato padre
        CASE WHEN n.TipoNegocioId = 3 THEN nPadre.BolsaId        ELSE n.BolsaId          END AS BolsaId,
        CASE WHEN n.TipoNegocioId = 3 THEN bolsaPadre.Descripcion ELSE bolsa.Descripcion  END AS Bolsa,

        n.Precio,
        CASE
            WHEN LTRIM(RTRIM(n.MonedaId)) = 'ARP'  THEN 'ARP'
            WHEN LTRIM(RTRIM(n.MonedaId)) = 'USDM' THEN 'USD'
            ELSE ''
        END AS Moneda,

        bd.FechaGeneracion,
        n.FechaOperacion,
        n.Fecha             AS FechaCarga,
        n.FechaConfirmacion,
        bd.FechaAnulacion,

        ISNULL(corredor.RazonSocial,  '') AS Corredor,
        ISNULL(proveedor.RazonSocial, '') AS Vendedor,
        n.MaterialId,
        mat.Descripcion                   AS Material,
        n.ContratoVendedor,
        n.ContratoCorredor,
        com.Apellido + ', ' + com.Nombres AS Comercial,
        n.FechaConfirmadoSAP,
        n.ComercialId,
        n.ProveedorId

    FROM Negocio n

    -- Contrato padre (solo para FijacionDePrecioContrato)
    LEFT JOIN Negocio nPadre           ON n.TipoNegocioId = 3 AND n.ContratoId = nPadre.Id

    -- Bolsa del negocio y del padre
    LEFT JOIN BolsaCompraNet bolsa      ON n.BolsaId       = bolsa.Id
    LEFT JOIN BolsaCompraNet bolsaPadre ON nPadre.BolsaId  = bolsaPadre.Id

    -- BoletoCompraNet del negocio y del padre
    LEFT JOIN BoletoCompraNet bc        ON n.BoletoId      = bc.Id
    LEFT JOIN BoletoCompraNet bcPadre   ON nPadre.BoletoId = bcPadre.Id

    -- Material
    LEFT JOIN Material mat              ON n.MaterialId    = mat.MaterialId    -- verificar PK de Material

    -- Comercial
    LEFT JOIN Comercial com             ON n.ComercialId   = com.ComercialId

    -- Corredor / Vendedor
    LEFT JOIN Proveedor corredor        ON n.CorredorId    = corredor.ProveedorId
    LEFT JOIN Proveedor proveedor       ON n.ProveedorId   = proveedor.ProveedorId
    -- TipoNegocio
    LEFT JOIN TipoNegocio tn            ON n.TipoNegocioId = tn.TipoNegocioId

    -- Último boleto
    LEFT JOIN BoletoDetalle bd          ON n.Id            = bd.NegocioId

    WHERE
        n.ConfirmadoSAP = 1
        AND n.EstadoId  = 5   -- EnumEstadoContrato.Finalizado
        AND (
            n.BoletoId = 2    -- FISICO
            OR n.BoletoId = 4 -- CARTA_OFERTA
        )
        -- Filtro de equipo (siempre obligatorio)
        AND (
            (n.ComercialId        IS NOT NULL AND n.ComercialId        IN (SELECT ComercialId FROM #Equipo))
            OR
            (n.ComercialCreadorId IS NOT NULL AND n.ComercialCreadorId IN (SELECT ComercialId FROM #Equipo))
        )

        -- ── Bloque NegocioSAP ──────────────────────────────────────────────────────
        AND (
            @NegocioSAP IS NULL
            OR n.ContratoSAP IN (SELECT ContratoSAP FROM #NegocioSAP)
            OR (n.TipoNegocioId = 3 AND n.FijacionSAP IN (SELECT ContratoSAP FROM #NegocioSAP))
        )
        -- Excluir fijaciones cuando no se busca por SAP
        AND (@NegocioSAP IS NOT NULL OR n.TipoNegocioId <> 3)

        -- ── Filtros opcionales (solo cuando no se busca por SAP) ──────────────────
        AND (@NegocioSAP IS NOT NULL OR @MaterialId IS NULL
             OR n.MaterialId = @MaterialId)
        AND (@NegocioSAP IS NOT NULL OR @ProveedorId IS NULL
             OR n.ProveedorId = @ProveedorId)
        AND (@NegocioSAP IS NOT NULL OR @ComercialId IS NULL
             OR n.ComercialId = @ComercialId)
        AND (@NegocioSAP IS NOT NULL OR @BolsaCompraNetId IS NULL
             OR n.BolsaId = @BolsaCompraNetId
             OR (n.TipoNegocioId = 3 AND nPadre.BolsaId = @BolsaCompraNetId))
        AND (@NegocioSAP IS NOT NULL OR @FechaConfirmacionDesde IS NULL
             OR n.FechaConfirmacion >= @FechaConfirmacionDesde)
        AND (@NegocioSAP IS NOT NULL OR @FechaConfirmacionHasta IS NULL
             OR n.FechaConfirmacion <= DATEADD(DAY, 1, CAST(@FechaConfirmacionHasta AS DATE)))

    ORDER BY n.Id DESC;

    DROP TABLE #Equipo;
    DROP TABLE #NegocioSAP;
END;
GO
