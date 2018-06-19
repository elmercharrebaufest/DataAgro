create procedure [dbo].[DataAgro_ExportAll_Almacenamiento] 
 @Proveedores VARCHAR(max)
as


declare @table as table(item int )

 insert into @table 
 select item FROM dbo.Split(@Proveedores, ',') 

SELECT
		pr.cuit as cuit,
		pr.razonsocial,
		L.Nombre Localidad,
		P.Nombre Provincia,
		CA.Descripcion CampañaPlanta,
		cast(AC.Toneladas  as varchar(20)) as CapacidadPlantaTn,
		case when ac.HasArrendadas = 1 then 'X' else '' end as 'Alquiladas',
		case when ac.HasArrendadas = 0 then 'X' else '' end as 'Propias',
		C.Descripcion Campaña,
		M.Descripcion as Material,
		cast(AM.Toneladas  as varchar(20)) as Toneladas,
		cast(pr.AlmacVolAnualTotal  as varchar(20)) as VolumenAnualTn,
		case when pr.AlmacHabilitadoSojaSust = 1 then 'SI' else '' end as 'HabilitadoSojaSustentable'

	FROM Acopio A  
	LEFT JOIN AcopioMaterial AM ON AM.AcopioId = A.AcopioId
	LEFT JOIN AcopioCampaña AC ON AC.AcopioId = A.AcopioId
	LEFT JOIN Proveedor PR ON A.ProveedorId = PR.ProveedorId
	LEFT JOIN Campaña C ON C.CampañaId = AM.CampañaId
	LEFT JOIN Material M ON M.MaterialId = AM.MaterialId
	LEFT JOIN Campaña CA ON CA.CampañaId = AC.CampañaId
	LEFT JOIN Localidad L ON L.LocalidadId = A.LocalidadId
	LEFT JOIN Provincia P ON P.ProvinciaId = L.ProvinciaId
	WHERE A.ProveedorId  in (select * from @table)