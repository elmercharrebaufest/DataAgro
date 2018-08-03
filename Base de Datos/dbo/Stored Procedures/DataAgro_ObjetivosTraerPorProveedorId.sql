
CREATE PROCEDURE [dbo].[DataAgro_ObjetivosTraerPorProveedorId] --26
(
	@ProveedorId INT
)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
		CM.CampañaId,
		C.Descripcion Campaña,
		CM.ObjetivoId,
		CM.MaterialId,
		M.Descripcion Material,
		CM.NroItem,
		CM.ProveedorId,
		CM.ToneladasObjetivos
	FROM Objetivo CM
	INNER JOIN Material M ON M.MaterialId = CM.MaterialId AND M.CampañaId <= CM.CampañaId
	INNER JOIN Campaña C ON C.CampañaId = CM.CampañaId
	WHERE CM.ProveedorId = @ProveedorId
    
END
