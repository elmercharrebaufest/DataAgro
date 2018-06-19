CREATE PROCEDURE [dbo].[DataAgro_BusquedaProveedores]
@filtro varchar(100) 

  
as  

BEGIN
		select p.ProveedorId as Id,p.RazonSocial,p.CUIT  
		from Proveedor p  
		inner join ProveedorComercial pc on p.ProveedorId= pc.ProveedorId  
		left join ContactoComercial cc on p.ProveedorId = cc.ProveedorId 
		where   
		CUIT like Replace(Replace(@filtro,'-',''),'.','') + '%'   
		or RazonSocial like @filtro + '%' 
		or cast(rtrim(Ltrim(cc.Apellido)) + ' ' + rtrim(Ltrim(cc.Nombres)) as varchar(105))  like @filtro + '%' 
		or cast(rtrim(Ltrim(cc.Nombres)) + ' ' + rtrim(Ltrim(cc.Apellido)) as varchar(105))  like @filtro + '%' 
		group by p.ProveedorId,p.RazonSocial,p.CUIT

END
