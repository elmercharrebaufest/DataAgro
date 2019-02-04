
--Se agrego como un listado en AcopioMaterialPorProveedores
CREATE PROCEDURE [dbo].[DataAgro_AcopioMaterialPorProveedorId] --3761
(
	 @ProveedorId INT
)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		AM.AcopioId,
		AM.AcopioMaterialId,
		AM.CampañaId,
		C.Descripcion Campaña,
		AM.MaterialId,
		AM.NroItem,
		AM.Toneladas,
		M.Descripcion Material,
		A.LocalidadId,
		L.Nombre Localidad,
		L.ProvinciaId,
		P.Nombre Provincia
	FROM AcopioMaterial AM
	LEFT JOIN  Acopio A ON AM.AcopioId = A.AcopioId
	LEFT JOIN Proveedor PR ON A.ProveedorId = PR.ProveedorId
	LEFT JOIN Campaña C ON C.CampañaId = AM.CampañaId
	LEFT JOIN Material M ON M.MaterialId = AM.MaterialId
	LEFT JOIN Localidad L ON L.LocalidadId = A.LocalidadId
	LEFT JOIN Provincia P ON P.ProvinciaId = L.ProvinciaId
	WHERE A.ProveedorId = @ProveedorId    
END
