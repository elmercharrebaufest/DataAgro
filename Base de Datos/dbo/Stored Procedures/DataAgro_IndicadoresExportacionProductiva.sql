
CREATE PROCEDURE [dbo].[DataAgro_IndicadoresExportacionProductiva]  

 @ProvinciaId int   ,

@SegmentacionId int    ,

@MaterialId int   ,

@CampañaId int    

as


create table #Valores(Prov varchar(200),Cl varchar(200),Tn Float )

 

insert into #Valores(Prov,Cl)

select distinct prv.Nombre, p.cuit 

from proveedor p

inner join Campo cp on p.ProveedorId = cp.ProveedorId

inner join CampoMaterial cm on cp.CampoId = cm.CampoId

inner join Localidad loc on cp.LocalidadId=loc.LocalidadId

inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 


where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))

and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))

and ( (@SegmentacionId is null) or (p.SegmentacionId= @SegmentacionId))

and ((@ProvinciaId is null) or (cp.LocalidadId = @ProvinciaId))

and cp.LocalidadId is not null and prv.ProvinciaId is not null


 

update #Valores

set Tn= b.tn
from
(select  p.cuit, sum (cm.Toneladas) as Tn 

from proveedor p

inner join Campo cp on p.ProveedorId = cp.ProveedorId

inner join CampoMaterial cm on cp.CampoId = cm.CampoId

inner join Localidad loc on cp.LocalidadId=loc.LocalidadId

inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 

 
where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))


and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))

and ( (@SegmentacionId is null) or (p.SegmentacionId= @SegmentacionId))

and ((@ProvinciaId is null) or (cp.LocalidadId = @ProvinciaId))

and cp.LocalidadId is not null and prv.ProvinciaId is not null






group by p.cuit )  b

where #Valores.Cl= b.CUIT

 

select    tn  as Tonelada, CL   as Cuit

from #Valores

 

 
 

drop table #Valores