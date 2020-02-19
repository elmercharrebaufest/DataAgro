CREATE PROCEDURE [dbo].[DataAgro_BasicoProveedorTraerPorCuit]
(
			@Cuit varchar(20) = 0
		
)


AS
BEGIN
	SET NOCOUNT ON;
		SELECT 
		Proveedor.ProveedorId,
		Proveedor.CUIT, 
		Proveedor.RazonSocial,
		Localidad.LocalidadId,
		Provincia.ProvinciaId,
		Localidad.Nombre as Localidad,
		Provincia.Nombre as Provincia
		from Proveedor
		INNER JOIN Localidad on Localidad.LocalidadId = Proveedor.LocalidadId
		INNER JOIN Provincia on Provincia.ProvinciaId = Localidad.ProvinciaId
			WHERE CUIT like  '%' + @Cuit +  '%'

END
GO
