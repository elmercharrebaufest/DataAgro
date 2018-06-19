  
Create procedure  [dbo].[DataAgro_BusquedaHome]  --'Dow',42
  
 @filtro varchar(100),  
 @ComercialId int  
  
as  
  
--WITH Empleados   
--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)  
--AS  
--(  
-- SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory  
--    FROM Comercial    
-- WHERE ComercialId = @comercialId   
-- UNION ALL   
--    --RECURSIVIDAD  
-- SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory  
-- FROM Comercial A  
-- inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId  
--)  

  declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
  insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId
  
  
select p.ProveedorId as Id,p.RazonSocial,p.CUIT  
from Proveedor p  
inner join ProveedorComercial pc on p.ProveedorId= pc.ProveedorId  
inner join @EmpleadoTable e on e.ComercialId = pc.ComercialId 
left join ContactoComercial cc on p.ProveedorId = cc.ProveedorId 
where   
CUIT like Replace(Replace(@filtro,'-',''),'.','') + '%'   
or RazonSocial like @filtro + '%' 
or cast(rtrim(Ltrim(cc.Apellido)) + ' ' + rtrim(Ltrim(cc.Nombres)) as varchar(105))  like @filtro + '%' 
or cast(rtrim(Ltrim(cc.Nombres)) + ' ' + rtrim(Ltrim(cc.Apellido)) as varchar(105))  like @filtro + '%' 
group by p.ProveedorId,p.RazonSocial,p.CUIT