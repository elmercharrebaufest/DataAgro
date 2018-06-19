
CREATE procedure [dbo].DataAgro_ExportAll_Produccion 

	@Proveedores VARCHAR(max)

as

declare @table as table(item int )

insert into @table 
select item FROM dbo.Split(@Proveedores, ',') 

SELECT 
	pr.CUIT,
	pr.RazonSocial,
	p.Nombre as Provincia,
	l.Nombre as localidad,
	M.Descripcion as Material,
	cam.Descripcion as Campaña,
	cast(cm.Hectareas as varchar(20)) as Hectareas,
	cast(cm.Toneladas as varchar(20)) as Toneladas,
	case when c.ArrendaPropia = 1 then 'X' else '' end as 'Propias',
	case when c.ArrendaPropia = 0 then 'X' else '' end as 'Alquiladas'

FROM Campo C
INNER JOIN Proveedor PR ON C.ProveedorId = PR.ProveedorId
LEFT JOIN CampoMaterial CM ON CM.CampoId = C.CampoId
LEFT JOIN Localidad L ON L.LocalidadId = C.LocalidadId
LEFT JOIN Provincia P ON P.ProvinciaId = L.ProvinciaId
LEFT JOIN Material M ON M.MaterialId = CM.MaterialId
LEFT JOIN Campaña CAM ON CAM.CampañaId = CM.CampañaId
WHERE C.ProveedorId  in (select * from @table)