create procedure [dbo].[DataAgro_ExportAll_Compras] 
 @Proveedores VARCHAR(max)
as


declare @table as table(item int )

 insert into @table 
 select Item FROM dbo.Split(@Proveedores, ',') 

	  
SELECT
 pr.CUIT as cuit,  
 pr.razonsocial,  
 C.Descripcion Campaña,  
 M.Descripcion as Material,  
 cast(ToneladaAplicada  as varchar(20)) as Toneladas,  
 case MONTH(cmm.FechaDesde) when 1 then 'Enero'   
    when 2 then 'Febrero'  
    when 3 then 'Marzo'  
    when 4 then 'Abril'  
    when 5 then 'Mayo'  
    when 6 then 'Junio'  
    when 7 then 'Julio'  
    when 8 then 'Agosto'  
    when 9 then 'Septiembre'  
    when 10 then 'Octubre'  
    when 11 then 'Noviembre'  
    when 12 then 'Diciembre'  
    else ''  
 end as Mes,  
 cast(YEAR(cmm.FechaDesde)  as varchar(20)) as Año  
from CampanaMaterialDetallePorMes cmm   
    inner join Proveedor pr on pr.ProveedorId = cmm.ProveedorId  
    LEFT JOIN Campaña C ON C.CampañaId = cmm.CampanaId  
    LEFT JOIN Material M ON M.MaterialId = Cmm.MaterialId  
WHERE PR.ProveedorId  in (select * from @table)