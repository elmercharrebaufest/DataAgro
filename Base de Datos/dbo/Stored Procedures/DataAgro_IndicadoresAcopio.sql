
create PROCEDURE [dbo].[DataAgro_IndicadoresAcopio]  

  @ProvinciaId int = null  ,
 @SegmentacionId VARCHAR(max) ,
 @MaterialId int = null  ,
 @CampañaId int  = null,
 @ComercialId int  =null

as

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int);
 
declare @ProvinciaSecuencia TABLE (Item INT) 

if (@ProvinciaId is not null)
begin 
	if (@ProvinciaId = 0 or @ProvinciaId = 1)
	begin 
	   insert into @ProvinciaSecuencia (item) values(0)
	   insert into @ProvinciaSecuencia (item) values(1)
	end
	else
		insert into @ProvinciaSecuencia (item) values(@ProvinciaId)

end

declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;
   
--RECURSIVIDAD
--WITH Empleados 
--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory,GrupoDeCompras)
--AS
--(
--	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory,GrupoDeCompras
--    FROM Comercial  
--	WHERE ComercialId = @comercialId 
--	UNION ALL 
--	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory,a.GrupoDeCompras
--	FROM Comercial A
--	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId
--)

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId

create table #Valores(Prov varchar(200),Cl bigint,Tn Float)

 

insert into #Valores(Prov,Cl)

select distinct prv.Nombre, p.cuit 

from proveedor p

inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId

inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId

inner join Acopio cp on p.ProveedorId = cp.ProveedorId

inner join AcopioMaterial cm on cp.AcopioId= cm.AcopioId

inner join Localidad loc on cp.LocalidadId=loc.LocalidadId

inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId


where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))

and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))

and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and (( @ProvinciaId is null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.ProvinciaId)))

and cp.LocalidadId is not null and prv.ProvinciaId is not null


 

update #Valores


set Tn= b.tn
from
(select  p.cuit,prv.Nombre as Prov, sum (cm.Toneladas) as Tn 

from proveedor p

inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId

inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId

inner join Acopio cp on p.ProveedorId = cp.ProveedorId

inner join AcopioMaterial cm on cp.AcopioId= cm.AcopioId

inner join Localidad loc on cp.LocalidadId=loc.LocalidadId

inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId


where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))

and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))

and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and (( @ProvinciaId is null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.ProvinciaId)))

and cp.LocalidadId is not null and prv.ProvinciaId is not null


group by p.cuit ,prv.Nombre)  b

where #Valores.Cl= b.CUIT and #Valores.Prov = b.Prov
 

select  Prov  as Provincia, sum(tn) as Tonelada,count(CL) as Cuit

from #Valores

group by prov  

drop table #Valores