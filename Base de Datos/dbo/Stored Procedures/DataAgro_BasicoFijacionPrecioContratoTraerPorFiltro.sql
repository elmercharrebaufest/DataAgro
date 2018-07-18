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
		(case when FijacionDePrecioContrato.Estado =1 then 'Pendiente' else
			((case when FijacionDePrecioContrato.Estado =2 then 'Confirmado' else
			((case when FijacionDePrecioContrato.Estado =3 then 'Oferta' else
			((case when FijacionDePrecioContrato.Estado =4 then 'Con Error' else 'Finalizado' end)) end)) end)) end) Estado,
		Observacion
		FROM FijacionDePrecioContrato
		INNER JOIN Proveedor on Proveedor.ProveedorId = FijacionDePrecioContrato.ProveedorId
		LEFT JOIN Comercial on Comercial.ComercialId = FijacionDePrecioContrato.ComercialId
		LEFT JOIN Material on Material.MaterialId = FijacionDePrecioContrato.MaterialId
		WHERE (FijacionDePrecioContrato.ContratoId = @ContratoId OR @ContratoId=0) 


END