
CREATE PROCEDURE DataAgro_CampañaMaterialPorMes_ToneladasActualizar

@cosecha varchar(50),
@material int,
@CUIT varchar(50)

as

begin tran

declare @ProveedorId int = null , @MaterialId int = null, 
@CampañaId int = null, @CampañaMaterialId int = null, @total float = null
set @ProveedorId = ( select ProveedorId from Proveedor where CUIT = @CUIT )
set @MaterialId = ( select MaterialId from Material where Codigo = @material )
set @CampañaId = ( select CampañaId from Campaña where Descripcion = @cosecha )
set @CampañaMaterialId = (select CampañaMaterialId
							from CampañaMaterial 
							where CampañaId=@CampañaId and ProveedorId = @ProveedorId and MaterialId = @MaterialId)
							


set @total =(select SUM(Toneladas) from CampañaMaterialPorMes where CampañaMaterialId = @CampañaMaterialId) 							

update dbo.CampañaMaterial
set ToneladasCompradas = @total
where CampañaMaterialId = @CampañaMaterialId


if (@@ERROR = 0 ) 
begin 
	commit tran
end
else
begin
 rollback tran
end
