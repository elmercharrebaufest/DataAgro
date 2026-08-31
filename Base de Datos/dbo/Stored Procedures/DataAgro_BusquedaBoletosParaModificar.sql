CREATE PROCEDURE [dbo].[DataAgro_BusquedaBoletosParaModificar]
    @ContratoSAP NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ContratosFiltro AS
    (
        SELECT LTRIM(RTRIM(value)) AS ContratoSAP
        FROM STRING_SPLIT(@ContratoSAP, ';')
        WHERE LTRIM(RTRIM(value)) <> ''
    )
    SELECT
        cb.Id AS ControlDeBoletosId,
        cb.NegocioId,
        ISNULL(p.ProveedorId, 0) AS ProveedorId,
        p.CUIT AS CUITProveedor,
        ISNULL(cor.ProveedorId, 0) AS CorredorId,
        cor.CUIT AS CUITCorredor,
        CAST(CASE WHEN n.CorredorId > 0 THEN 1 ELSE 0 END AS bit) AS EsCorredor,
        b.Descripcion AS TipoBoleto,
        CAST(CASE WHEN n.BoletoId = 4 THEN 1 ELSE 0 END AS bit) AS EsCartaOferta,
        CAST(CASE WHEN n.BoletoId = 5 THEN 1 ELSE 0 END AS bit) AS EsSinBoleto,
        CAST(0 AS bit) AS OperaSinOblea,
        n.ContratoSAP,
        pre.Id AS PreCertificacionId,
        pre.Oblea,
        pre.BolsaCompraNetId AS PreCertificacionBolsaCompraNetId,
        bolsaPre.CodigoSap AS PreCertificacionBolsa,
        pre.FechaCertificacion,
        pre.FechaVencimiento AS FechaVencimientoCertificacion,
        seg.BolsaCompraNetId,
        seg.BolsaSellado,
        seg.Id AS SeguimientoBoletoId,
        seg.FechaRecepcionBoleto AS FechaRecepBoleto,
        seg.FechaEnvioFirma AS FechaEnviadoFirma,
        seg.FechaEnvioBolsa,
        seg.FechaEnvioAfip,
        seg.FechaRecepcionFirma AS FechaRecibFirma,
        seg.FechaRecepcionBolsa AS FechaVueltaBolsa,
        seg.FechaRecepcionAfip AS FechaVueltaAfip,
        seg.FechaEnvioSellado,
        seg.FechaEnvioFisicoBolsa AS FechaEnvioFisicoBolsa,
        seg.FechaRecepcionBoletoOriginal AS FechaRecepcionBoletoOriginal
    FROM ControlDeBoletos cb
    INNER JOIN Negocio n ON n.Id = cb.NegocioId
    INNER JOIN ControlDeBoletosSeguimiento seg ON seg.ControlDeBoletosId = cb.Id
    LEFT JOIN BoletoCompraNet b ON b.Id = n.BoletoId
    LEFT JOIN Proveedor p ON p.ProveedorId = n.ProveedorId
    LEFT JOIN Proveedor cor ON cor.ProveedorId = n.CorredorId
    LEFT JOIN ControlDeBoletosPreCertificacion pre ON pre.ControlDeBoletosId = cb.Id and pre.TipoObleaId = 3
    LEFT JOIN BolsaCompraNet bolsaPre ON bolsaPre.Id = pre.BolsaCompraNetId
    WHERE EXISTS (
                SELECT 1
                FROM ContratosFiltro f
                WHERE f.ContratoSAP = n.ContratoSAP
            )
    ORDER BY cb.Id;
END;
