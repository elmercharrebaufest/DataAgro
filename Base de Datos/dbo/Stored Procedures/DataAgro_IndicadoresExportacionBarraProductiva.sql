CREATE PROCEDURE [dbo].[DataAgro_IndicadoresExportacionBarraProductiva]  

 @SegmentacionId VARCHAR(max) ,
 @MaterialId int = null,
 @CampañaId int = null,
 @ComercialId int =null,
 @ComercialGenerador VARCHAR(max)=null 
 
AS

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);

 declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador;

create table #Valores(Criterio varchar(100),Toneladas float,Cuit varchar(100),Material varchar(100),Campaña varchar(100),PropiaAlquilada varchar(100),
SojaSustentable varchar(100), Segmentación varchar(100),Provincia varchar(100),Comercial varchar(100),razonSocial varchar(100))


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,PropiaAlquilada,SojaSustentable,Segmentación,Comercial,razonSocial)
select 'Mas de 20000 TN.',p.cuit,cm.toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when cp.ArrendaPropia= 1 then 'Propia' else 'Alquilada' end 'PropiaAlquilada',
case when cp.HabilitadoSojaSustentable= 1 then 'Si' else 'No' end 'SojaSustentable',
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.razonSocial

from CampoMaterial cm
inner join Campo cp on cp.CampoId=cm.CampoId
inner join Proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Segmentacion seg on p.segmentacionId=seg.segmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.cuit in (
select distinct  p.cuit
from CampoMaterial cm
inner join campo cp on cp.CampoId=cm.CampoId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.cuit
having sum(cm.toneladas) > 20000 )

ORDER BY P.CUIT


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,PropiaAlquilada,SojaSustentable,Segmentación,Comercial,razonSocial)
select 'Entre 10000 y 20000 TN.',p.cuit,cm.toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when cp.ArrendaPropia= 1 then 'Propia' else 'Alquilada' end 'PropiaAlquilada',
case when cp.HabilitadoSojaSustentable= 1 then 'Si' else 'No' end 'SojaSustentable',
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.razonsocial

from CampoMaterial cm
inner join campo cp on cp.CampoId=cm.CampoId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join segmentacion seg on p.segmentacionId=seg.segmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.cuit in (
select distinct  p.cuit
from CampoMaterial cm
inner join campo cp on cp.CampoId=cm.CampoId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.cuit
having sum(cm.toneladas) between 10000 and 20000 )

ORDER BY P.CUIT


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,PropiaAlquilada,SojaSustentable,Segmentación,Comercial,razonSocial)
select 'Entre 5000 y 10000 TN.',p.cuit,cm.toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when cp.ArrendaPropia= 1 then 'Propia' else 'Alquilada' end 'PropiaAlquilada',
case when cp.HabilitadoSojaSustentable= 1 then 'Si' else 'No' end 'SojaSustentable',
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.razonsocial

from CampoMaterial cm
inner join campo cp on cp.CampoId=cm.CampoId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join segmentacion seg on p.segmentacionId=seg.segmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.cuit in (
select distinct  p.cuit
from CampoMaterial cm
inner join campo cp on cp.CampoId=cm.CampoId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.cuit
having sum(cm.toneladas) between 5000 and 10000 )

ORDER BY P.CUIT


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,PropiaAlquilada,SojaSustentable,Segmentación,Comercial,razonSocial)
select 'Menos de 5000 TN.',p.cuit,cm.toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when cp.ArrendaPropia= 1 then 'Propia' else 'Alquilada' end 'PropiaAlquilada',
case when cp.HabilitadoSojaSustentable= 1 then 'Si' else 'No' end 'SojaSustentable',
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.razonSocial

from CampoMaterial cm
inner join campo cp on cp.CampoId=cm.CampoId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join segmentacion seg on p.segmentacionId=seg.segmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.cuit in (
select distinct  p.cuit
from CampoMaterial cm
inner join campo cp on cp.CampoId=cm.CampoId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.cuit
having sum(cm.toneladas) < 5000 )

ORDER BY P.CUIT

select * from #Valores


drop table #Valores