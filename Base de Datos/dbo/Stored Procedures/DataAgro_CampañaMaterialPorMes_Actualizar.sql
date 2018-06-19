
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
	if (select COUNT(CampañaMaterialId) from CampañaMaterial) > 0
		set @CampañaMaterialId = (select MAX(CampañaMaterialId)+1 from CampañaMaterial)
	else
		set @CampañaMaterialId = 1
		
	insert into	CampañaMaterial(CampañaMaterialId,CampañaId, NroItem, ProveedorId, MaterialId, ToneladasCompradas)
	 values (@CampañaMaterialId,@CampañaId,1,@ProveedorId,@MaterialId,0)	 
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

	declare @CampañaMaterialPorMesId int
	
	if (select COUNT(CampañaMaterialPorMesId) from CampañaMaterialPorMes ) > 0
	begin 
		set @CampañaMaterialPorMesId = (select MAX(CampañaMaterialPorMesId)+1 from CampañaMaterialPorMes)
	end 
	else
	begin 
		set @CampañaMaterialPorMesId = 1
	end
	
	insert into CampañaMaterialPorMes(CampañaMaterialPorMesId, NroItem, Mes, Toneladas, CampañaMaterialId, Año, ComercialId) 
	 values(@CampañaMaterialPorMesId,1,@mes,@tnComprados,@CampañaMaterialId,@anio,@ComercialId)
	 
end

if (@@ERROR = 0 ) 
begin 
	commit tran
end
else
begin
 rollback tran
end