CREATE PROCEDURE [dbo].[DataAgro_BasicoContratoTraerPorFiltro]
(
			@ProveedorId int = 0,
			@ContratoSap int = 0,
			@ComercialId int = 0,
			@MaterialId int = 0,
			@Precio  float ,
			@CampaniaId int = 0,
			@FechaDesde varchar(10),
			@FechaHasta varchar(10),
			@EstadoId int = 0,
			@Usuario varchar(100)
)


AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		Contrato.ContratoId,
		Contrato.MaterialId,
		Contrato.TipoNegocioId,
		Contrato.Cantidad,
		Contrato.Precio,
		(cast(Contrato.FechaEntrega as varchar(12))) FechaEntrega, 
		Contrato.CampañaId,
		(cast(Contrato.FechaDesde as varchar(12)))  FechaDesde,
		(cast(Contrato.FechaHasta as varchar(12)))  FechaHasta,
		Contrato.ProveedorId,
		Contrato.MonedaId,
		(cast(Contrato.Fecha as varchar(12))) Fecha,
		Contrato.GrupoCompra,
		Contrato.ComercialId,
		Contrato.ProvinciaId,
		Contrato.LocalidadId,
		Contrato.Base,
		Contrato.Importe_Sustentable,
		Contrato.MonedaId_Sustentable,
		(cast(Contrato.Fecha_Dolarizado as varchar(12))) Fecha_Dolarizado, 
		Contrato.Dias_Pesificado,
		Contrato.NoInformaSIO,
		TrigoEspecial,
		Contrato.Estado,
		Contrato.UsuarioId,
		Contrato.ContratoSAP,
		Contrato.Ampliaciones,
		Proveedor.RazonSocial Proveedor,
		Comercial.Nombres as Comercial,
		Material.Descripcion as Material,
		Campaña.Descripcion Campania,
		Provincia.Nombre as Provincia,
		TipoNegocio.Descripcion as TipoNegocio,
		 Localidad.Nombre Localidad,
		(case when Contrato.Estado =1 then 'Pendiente' else
			((case when Contrato.Estado =2 then 'Confirmado' else
			((case when Contrato.Estado =3 then 'Oferta' else
			((case when Contrato.Estado =4 then 'Con Error' else 'Finalizado' end)) end)) end)) end) Estado_Contrato,
		FijacionDePrecioContrato.Cantidad Cantidad_F,
		FijacionDePrecioContrato.Precio Precio_F,
		Proveedor_F.RazonSocial Proveedor_F,
		(cast(FijacionDePrecioContrato.Fecha as varchar(12)))  Fecha_F,
		Comercial_F.Nombres as Comercial_F,
		Material_F.Descripcion as Material_F,
		Proveedor_F.NombreReferente as Proveedor_F,
		FijacionDePrecioContrato.MonedaId MonedaId_F,
		FijacionDePrecioContrato.Ampliaciones Ampliaciones_F,
		(case when FijacionDePrecioContrato.Estado =1 then 'Pendiente' else
			((case when FijacionDePrecioContrato.Estado =2 then 'Confirmado' else
			((case when FijacionDePrecioContrato.Estado =3 then 'Oferta' else
			((case when FijacionDePrecioContrato.Estado =4 then 'Con Error' else 'Finalizado' end)) end)) end)) end) Estado_F
		FROM Contrato
		LEFT JOIN FijacionDePrecioContrato on FijacionDePrecioContrato.ContratoId = Contrato.ContratoId
		LEFT JOIN Proveedor on Proveedor.ProveedorId = Contrato.ProveedorId
		LEFT JOIN Comercial on Comercial.ComercialId = Contrato.ComercialId
		LEFT JOIN Material on Material.MaterialId = Contrato.MaterialId
		LEFT JOIN TipoNegocio on TipoNegocio.TipoNegocioId = Contrato.TipoNegocioId
		LEFT JOIN Monedas on Monedas.MonedaId = Contrato.MonedaId
		LEFT JOIN Campaña on Campaña.CampañaId = Contrato.CampañaId
		LEFT JOIN Localidad on Localidad.LocalidadId = Contrato.LocalidadId
		LEFT JOIN Provincia on Provincia.ProvinciaId = Contrato.ProvinciaId
		LEFT JOIN Proveedor Proveedor_F on Proveedor_F.ProveedorId = FijacionDePrecioContrato.ProveedorId
		LEFT JOIN Comercial Comercial_F on Comercial_F.ComercialId = FijacionDePrecioContrato.ComercialId
		LEFT JOIN Material Material_F on Material_F.MaterialId = FijacionDePrecioContrato.MaterialId

		WHERE (Contrato.ProveedorId = @ProveedorId OR @ProveedorId=0) AND
			  (Contrato.ContratoSAP = @ContratoSap OR @ContratoSap=0) AND
			  (Contrato.ComercialId = @ComercialId OR @ComercialId=0) AND
			  (Contrato.MaterialId = @MaterialId OR @MaterialId=0) AND
			  (Contrato.Precio >= @Precio OR @Precio=0) AND
			  (Contrato.CampañaId = @CampaniaId OR @CampaniaId=0) AND
			  (Contrato.Estado = @EstadoId OR @EstadoId=0) 
END