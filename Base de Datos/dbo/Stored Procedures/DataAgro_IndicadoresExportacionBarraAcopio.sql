
create PROCEDURE [dbo].[DataAgro_IndicadoresExportacionBarraAcopio]  

  @SegmentacionId VARCHAR(max) ,
 @MaterialId int,
 @CampañaId int,
 @ComercialId int = 44  
 
as

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int);

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

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId;




create table #Valores(Criterio varchar(100),Toneladas float,Cuit varchar(100),Material varchar(100),Campaña varchar(100)
, Segmentación varchar(100),Provincia varchar(100),Comercial varchar(100),razonSocial varchar(100))


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,Segmentación,Comercial,razonSocial)
select 'Mas de 5000 TN.',p.cuit,cm.toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.RazonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
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
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.cuit in (
select distinct  p.cuit
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cmm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.cuit
having sum(cmm.toneladas) > 5000 )

ORDER BY P.CUIT
 
  
insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,Segmentación,Comercial,razonSocial)
select 'Entre 2500 y 5000 TN.',p.cuit,cm.toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.razonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
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
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.cuit in (
select distinct  p.cuit
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cmm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.cuit
having sum(cmm.toneladas) between 2500 and 5000 )

ORDER BY P.CUIT


insert into #Valores (Criterio,Cuit ,Toneladas ,Material,Campaña,Provincia,Segmentación,Comercial,razonSocial)
select 'Menos de 2500 TN.',p.cuit,cm.toneladas as Tonelada,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.razonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
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
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and p.cuit in (
select distinct  p.cuit
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cmm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
group by p.cuit
having sum(cmm.toneladas) < 2500 )
ORDER BY P.CUIT


select * from #Valores

drop table #Valores