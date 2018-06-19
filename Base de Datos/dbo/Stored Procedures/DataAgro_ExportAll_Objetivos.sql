create PROCEDURE [dbo].[DataAgro_ExportAll_Objetivos] --6
(
	@Proveedores VARCHAR(max)
)
AS


declare @table as table(item int )

insert into @table 
select item FROM dbo.Split(@Proveedores, ',') 

SELECT p.cuit as cuit,
	p.razonsocial,
	M.Descripcion Material,
	C.Descripcion Campaña,
	cast(CM.ToneladasObjetivos as varchar(1000)) as Tonelada
FROM Objetivo CM
INNER JOIN Material M ON M.MaterialId = CM.MaterialId AND M.CampañaIdActual <= CM.CampañaId
INNER JOIN Campaña C ON C.CampañaId = CM.CampañaId
INNER JOIN Proveedor p on CM.ProveedorId =p.ProveedorId
WHERE CM.ProveedorId in (select * from @table)