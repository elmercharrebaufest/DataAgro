create procedure [dbo].[DataAgro_ExportAll_Compras] 
 @Proveedores VARCHAR(max)
as


declare @table as table(item int )

 insert into @table 
 select item FROM dbo.Split(@Proveedores, ',') 

	select
	pr.cuit as cuit,
	pr.razonsocial,
	C.Descripcion Campaña,
	M.Descripcion as Material,
	cast(Toneladas  as varchar(20)) as Toneladas,
	case  when Mes= 1 then 'Enero' 
			 when Mes=2 then 'Febrero'
			 when Mes=3 then 'Marzo'
			 when Mes=4 then 'Abril'
			 when Mes=5 then 'Mayo'
			 when Mes=6 then 'Junio'
			 when Mes=7 then 'Julio'
			 when Mes=8 then 'Agosto'
			 when Mes=9 then 'Septiembre'
			 when Mes=10 then 'Octubre'
			 when Mes=11 then 'Noviembre'
			 when Mes=12 then 'Diciembre'
			 else ''
	end as Mes,
	cast(Año  as varchar(20)) as Año
	from CampañaMaterialPorMes cmm
	inner join CampañaMaterial cm on cm.CampañaMaterialId = cmm.CampañaMaterialId
	inner join Proveedor pr on pr.ProveedorId = cm.ProveedorId
	LEFT JOIN Campaña C ON C.CampañaId = CM.CampañaId
	LEFT JOIN Material M ON M.MaterialId = CM.MaterialId
	WHERE PR.ProveedorId  in (select * from @table)