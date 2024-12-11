CREATE PROCEDURE [dbo].[DataAgro_IndicadoresExportacionBarraAcopio]

 @SegmentacionId VARCHAR(max),
 @MaterialId int,
 @CampañaId int,
 @ComercialId int =null,
 @ComercialGenerador VARCHAR(max)=null
 
as

declare @EmpleadoTable TABLE (ComercialId int, Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);

declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;
  
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador;


create table #Valores(Criterio varchar(100),Toneladas float,Cuit varchar(100),Material varchar(100),Campaña varchar(100)
, Segmentación varchar(100),Provincia varchar(100),Comercial varchar(100),razonSocial varchar(100))


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,Segmentación,Comercial,razonSocial)
select 'Mas de 40000 TN55.',p.CUIT,cm.Toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.Grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.RazonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
inner join Proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Segmentacion seg on p.SegmentacionId=seg.SegmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@ComercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.CUIT in (
select distinct  p.CUIT
from Proveedor p
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cmm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.CUIT
having sum(cmm.Toneladas) > 40000 )

ORDER BY P.CUIT


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,Segmentación,Comercial,razonSocial)
select 'Entre 20000 y 40000 TN.',p.CUIT,cm.Toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.Grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.RazonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
inner join Proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Segmentacion seg on p.SegmentacionId=seg.SegmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.CUIT in (
select distinct  p.CUIT
from Proveedor p
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cmm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.CUIT
having sum(cmm.Toneladas) between 20000 and 40000 )

ORDER BY P.CUIT

 
  
insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,Segmentación,Comercial,razonSocial)
select 'Entre 10000 y 20000 TN.',p.CUIT,cm.Toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.Grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.RazonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
inner join Proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Segmentacion seg on p.SegmentacionId=seg.SegmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.CUIT in (
select distinct  p.CUIT
from Proveedor p
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cmm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.CUIT
having sum(cmm.Toneladas) between 10000 and 20000 )

ORDER BY P.CUIT


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,Segmentación,Comercial,razonSocial)
select 'Menos de 10000 TN.',p.CUIT,cm.Toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.Grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.RazonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
inner join Proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Segmentacion seg on p.SegmentacionId=seg.SegmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.CUIT in (
select distinct  p.CUIT
from Proveedor p
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cmm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.CUIT
having sum(cmm.Toneladas) < 10000 )
ORDER BY P.CUIT


select * from #Valores

drop table #Valores