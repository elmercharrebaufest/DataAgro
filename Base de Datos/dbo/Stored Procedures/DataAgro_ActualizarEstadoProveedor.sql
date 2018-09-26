CREATE proc [dbo].[DataAgro_ActualizarEstadoProveedor]

@ComercialId int , 
@CUIT varchar(50), 
@CLIENTEMOA bit, 
@EstadoId int

as

declare @proveedor int = 0

set @proveedor = (select ProveedorId from Proveedor where CUIT = @CUIT)

if ( @proveedor > 0 )
begin 
 
	if exists(select 1 from ProveedorEstado pe where pe.ProveedorId = @proveedor and pe.ComercialId = @ComercialId)
	begin
		update ProveedorEstado 
		set EstadoId = @EstadoId
		where ProveedorId = @proveedor and ComercialId = @ComercialId
	end
	else
	begin
		insert into ProveedorEstado(ProveedorId, EstadoId, ComercialId)
		values (@proveedor,@EstadoId,@ComercialId)

	end

	update Proveedor
	set ClienteMOA = @CLIENTEMOA
	where ProveedorId = @proveedor

end
 

select 1

GO


