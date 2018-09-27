
CREATE PROCEDURE [dbo].[DataAgro_CampañaMaterialPorMes_Actualizar]

@anio varchar(50),
@cosecha varchar(50),
@material int,
@tnComprados varchar(50),
@mes varchar(50),
@CUIT varchar(50),
@ComercialId int

as

begin tran

declare @ProveedorId int = null , @MaterialId int = null, @CampañaId int = null, @CampañaMaterialId int = null
set @ProveedorId = ( select ProveedorId from Proveedor where CUIT = @CUIT )
set @MaterialId = ( select MaterialId from Material where Codigo = @material )
set @CampañaId = ( select CampañaId from Campaña where Descripcion = @cosecha )

if not exists(select 1 from CampañaMaterial where CampañaId=@CampañaId and ProveedorId = @ProveedorId and MaterialId = @MaterialId )
begin
	insert into	CampañaMaterial(CampañaId, NroItem, ProveedorId, MaterialId, ToneladasCompradas)
	 values (@CampañaId,1,@ProveedorId,@MaterialId,0)	 
end
else
	set @CampañaMaterialId = (select CampañaMaterialId from CampañaMaterial where CampañaId=@CampañaId and ProveedorId = @ProveedorId and MaterialId = @MaterialId )

	if exists( select 1 from CampañaMaterialPorMes where CampañaMaterialId = @CampañaMaterialId and Mes= @mes and Año=@anio and ComercialId = @ComercialId)
	begin
		update CampañaMaterialPorMes
		set Toneladas = @tnComprados
		where CampañaMaterialId = @CampañaMaterialId and Mes= @mes and Año=@anio and ComercialId = @ComercialId
	end
	else
	begin

		insert into CampañaMaterialPorMes(NroItem, Mes, Toneladas, CampañaMaterialId, Año, ComercialId) 
		 values(1,@mes,@tnComprados,@CampañaMaterialId,@anio,@ComercialId)
	 
	end

if (@@ERROR = 0 ) 
begin 
	commit tran
end
else
begin
 rollback tran
end