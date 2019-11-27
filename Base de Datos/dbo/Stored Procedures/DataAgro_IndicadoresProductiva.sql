CREATE PROCEDURE [dbo].[DataAgro_IndicadoresProductiva]  

@ProvinciaId int   ,
@SegmentacionId VARCHAR(max) ,
@MaterialId int   ,
@CampañaId int ,   
@ComercialId int,
@ComercialGenerador VARCHAR(max)=null
  
as


declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);

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
  
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador
 
create table #Valores(Prov varchar(200),Cl bigint,Tn Float)

insert into #Valores(Prov,Cl)

select distinct prv.Nombre, p.cuit 

from proveedor p

inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId

inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId

inner join Campo cp on p.ProveedorId = cp.ProveedorId

inner join CampoMaterial cm on cp.CampoId = cm.CampoId

inner join Localidad loc on cp.LocalidadId=loc.LocalidadId

inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))

and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))

and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and (( @ProvinciaId is null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.ProvinciaId)))
and cp.LocalidadId is not null 
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))

update #Valores
set Tn= b.tn
from
(select  p.cuit,prv.Nombre as Prov, sum (cm.Toneladas) as Tn 
from proveedor p
inner join Campo cp on p.ProveedorId = cp.ProveedorId
inner join CampoMaterial cm on cp.CampoId = cm.CampoId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId=prv.ProvinciaId 
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))

and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and (( @ProvinciaId is null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.ProvinciaId)))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))


and cp.LocalidadId is not null
group by p.cuit,prv.Nombre )  b
where #Valores.Cl = b.CUIT and #Valores.Prov = b.Prov

select  Prov  as Provincia, sum(tn) as Tonelada,count(CL) as Cuit
from #Valores
group by prov  

drop table #Valores