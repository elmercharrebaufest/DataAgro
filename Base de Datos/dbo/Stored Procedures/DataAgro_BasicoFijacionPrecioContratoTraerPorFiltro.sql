CREATE PROCEDURE [dbo].[DataAgro_BasicoFijacionPrecioContratoTraerPorFiltro]
(
			
			@ContratoId int = 0
		
)


AS
BEGIN
	SET NOCOUNT ON;
		SELECT
		FijacionDePrecioContrato.FijacionDePrecioContratoId,
		FijacionDePrecioContrato.ContratoId,
		Proveedor.RazonSocial Proveedor,
		(cast(FijacionDePrecioContrato.Fecha as varchar(12)))  Fecha,
		Comercial.Nombres as Comercial,
		Material.Descripcion as Material,
		FijacionDePrecioContrato.Cantidad,
		Proveedor.NombreReferente as Proveedor,
		Material.Descripcion as Material,
		MonedaId,
		FijacionDePrecioContrato.Ampliaciones,
		EstadoContrato.Descripcion as Estado,
		Observacion
		FROM FijacionDePrecioContrato
		INNER JOIN Proveedor on Proveedor.ProveedorId = FijacionDePrecioContrato.ProveedorId
		LEFT JOIN Comercial on Comercial.ComercialId = FijacionDePrecioContrato.ComercialId
		LEFT JOIN Material on Material.MaterialId = FijacionDePrecioContrato.MaterialId
		INNER JOIN EstadoContrato on EstadoContrato.EstadoContratoId = FijacionDePrecioContrato.Estado
		WHERE (FijacionDePrecioContrato.ContratoId = @ContratoId OR @ContratoId=0) 


END