  
--YA ESTA EN REPOSITORIO = ConsultaBusquedaHome
Create procedure  [dbo].[DataAgro_BusquedaHome]  --'Dow',42
  
 @filtro varchar(100),  
 @ComercialId int  
  
as  
  

  declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)
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