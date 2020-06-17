
CREATE PROCEDURE [dbo].[DataAgro_CampoProduccionAcopioPorProveedorId]
(
	 @ProveedorId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CampoAcopio TABLE(
		Provincia varchar(100),
		ProvinciaId int,
		Localidad varchar(100),
		LocalidadId int,
		ArrendadoPropio bit,
		MaterialId int,
		Material varchar(100),
		HectareasPorcentaje float,
		Toneladas float,
		CampañaId int,
		Campaña varchar(100),
		AlmacVolAnualTotal float null,
		AlmacHabilitadoSojaSust bit null,
		AlmacTonsMaxSojaSust float null,
		AlmacHectSojaSust float null,
		EsCampoProduccion bit,
		Id int,
		HasArrendadas bit,
		KMZnombre varchar(500),
		KMZfile varchar(MAX),
		Partido varchar(500)
	)
	
	INSERT INTO @CampoAcopio
	SELECT 
		P.Nombre,
		P.ProvinciaId,
		L.Nombre,
		L.LocalidadId,
		C.ArrendaPropia,
		CM.MaterialId,
		M.Descripcion,
		CM.Hectareas,
		CM.Toneladas,
		CM.CampañaId,
		CAM.Descripcion,
		null as AlmacVolAnualTotal,
		null as AlmacHabilitadoSojaSust,
		PR.AlmacHectSojaSust,
		PR.AlmacTonsMaxSojaSust,
		1,
		C.CampoId,
		0,
		C.KMZnombre,
		c.KMZfile,
		Pa.Descripcion as Partido
	FROM Campo C
	INNER JOIN Proveedor PR ON C.ProveedorId = PR.ProveedorId
	LEFT JOIN CampoMaterial CM ON CM.CampoId = C.CampoId
	LEFT JOIN Localidad L ON L.LocalidadId = C.LocalidadId
	LEFT JOIN Provincia P ON P.ProvinciaId = L.ProvinciaId
	LEFT JOIN Partido PA ON PA.Id = L.PartidoId
	LEFT JOIN Material M ON M.MaterialId = CM.MaterialId
	LEFT JOIN Campaña CAM ON CAM.CampañaId = CM.CampañaId
	WHERE C.ProveedorId = @ProveedorId

	INSERT INTO @CampoAcopio
	SELECT
		P.Nombre,
		P.ProvinciaId,
		L.Nombre,
		L.LocalidadId,
		0,
		0,
		'',
		0,
		AM.Toneladas,
		AM.CampañaId,
		CAM.Descripcion,
		PR.AlmacVolAnualTotal,
		PR.AlmacHabilitadoSojaSust,
		null as AlmacHectSojaSust,
		null as AlmacTonsMaxSojaSust,
		0,
		A.AcopioId,
		AM.HasArrendadas,
		A.KMZnombre,
		A.KMZfile,
		Pa.Descripcion as Partido
	FROM Acopio A
	INNER JOIN Proveedor PR ON A.ProveedorId = PR.ProveedorId
	LEFT JOIN AcopioCampaña AM ON AM.AcopioId = A.AcopioId
	LEFT JOIN Localidad L ON L.LocalidadId = a.LocalidadId
	LEFT JOIN Provincia P ON P.ProvinciaId = L.ProvinciaId
	LEFT JOIN Partido PA ON PA.Id = L.PartidoId
	LEFT JOIN Campaña CAM ON CAM.CampañaId = AM.CampañaId
	WHERE A.ProveedorId = @ProveedorId

	SELECT
		Provincia,
		ProvinciaId,
		Localidad,
		LocalidadId,
		ArrendadoPropio,
		MaterialId,
		Material,
		HectareasPorcentaje,
		Toneladas,
		CampañaId,
		Campaña,
		AlmacVolAnualTotal,
		AlmacHabilitadoSojaSust,
		AlmacTonsMaxSojaSust,
		AlmacHectSojaSust ,
		EsCampoProduccion,
		Id,
		HasArrendadas,
		KMZnombre,
		KMZfile,
		Partido
	FROM @CampoAcopio	
    
END
