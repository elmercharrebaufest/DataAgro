CREATE procedure [dbo].[Reporte_ComprasBarra_TraerExcel]

 @Mes int= null,
 @SegmentacionId VARCHAR(max) ,
 @MaterialId int= null,
 @CampañaId int= null,
 @comercialId int =null,
 @ComercialGenerador VARCHAR(max)=null

as

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeCompras int);
   
declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador

create table #Valores(Toneladas float,Cuit varchar(100),Material varchar(100),Campaña varchar(100),Año varchar(100),Segmentación varchar(100),Provincia varchar(100),Comercial varchar(100),Mes varchar(100),Criterio varchar(100),razonSocial varchar(100))
insert into #Valores (Cuit ,Toneladas ,Material,Campaña,Año,Segmentación,Provincia,Comercial,Mes,Criterio,razonSocial )
select cast(p.cuit as varchar(100)) as CUIT,cmm.toneladas as Toneladas,m.Descripcion as Material,c.Descripcion as Campaña,cast(cmm.Año as varchar(20)) as Año,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end,isnull(prv.Nombre,'SIN PROVINCIA' )as Provincia,emp.Apellido + ' ' + emp.Nombres as Comercial
,case when cmm.Mes=1 then 'ENERO'
when cmm.Mes=2 then 'FEBRERO' 
when cmm.Mes=3 then 'MARZO' 
when cmm.Mes=4 then 'ABRIL' 
when cmm.Mes=5 then 'MAYO' 
when cmm.Mes=6 then 'JUNIO' 
when cmm.Mes=7 then 'JULIO' 
when cmm.Mes=8 then 'AGOSTO' 
when cmm.Mes=9 then 'SEPTIEMBRE' 
when cmm.Mes=10 then 'OCTUBRE' 
when cmm.Mes=11 then 'NOVIEMBRE' 
when cmm.Mes=12 then 'DICIEMBRE' 
else 'SIN MES'
end  as Mes,
'Mas de 10000 TN.',
p.RazonSocial as razonSocial

from CampañaMaterialPorMes cmm
inner join CampañaMaterial cm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join proveedor p on p.ProveedorId= cm.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
left join Localidad loc on p.LocalidadId= loc.LocalidadId
left join provincia prv on loc.ProvinciaId = prv.ProvinciaId
inner join Segmentacion seg on seg.segmentacionId=p.segmentacionId
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
and p.cuit in (
select distinct  p.cuit
from proveedor p
inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
group by p.cuit
having sum(cmm.toneladas) > 10000 )
order by p.cuit 

insert into #Valores (Cuit ,Toneladas ,Material,Campaña,Año,Segmentación,Provincia,Comercial,Mes,Criterio,razonSocial )
select cast(p.cuit as varchar(100)) as CUIT,cmm.toneladas as Toneladas,m.Descripcion as Material,c.Descripcion as Campaña,cast(cmm.Año as varchar(20)) as Año,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end,isnull(prv.Nombre,'SIN PROVINCIA' )as Provincia,emp.Apellido + ' ' + emp.Nombres as Comercial
,case when cmm.Mes=1 then 'ENERO'
when cmm.Mes=2 then 'FEBRERO' 
when cmm.Mes=3 then 'MARZO' 
when cmm.Mes=4 then 'ABRIL' 
when cmm.Mes=5 then 'MAYO' 
when cmm.Mes=6 then 'JUNIO' 
when cmm.Mes=7 then 'JULIO' 
when cmm.Mes=8 then 'AGOSTO' 
when cmm.Mes=9 then 'SEPTIEMBRE' 
when cmm.Mes=10 then 'OCTUBRE' 
when cmm.Mes=11 then 'NOVIEMBRE' 
when cmm.Mes=12 then 'DICIEMBRE' 
else 'SIN MES'
end  as Mes,
'Entre 5000 y 10000 TN.',
p.RazonSocial as razonSocial

from CampañaMaterialPorMes cmm
inner join CampañaMaterial cm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join proveedor p on p.ProveedorId= cm.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
left join Localidad loc on p.LocalidadId= loc.LocalidadId
left join provincia prv on loc.ProvinciaId = prv.ProvinciaId
inner join Segmentacion seg on seg.segmentacionId=p.segmentacionId
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
and p.cuit in (
select distinct  p.cuit
from proveedor p
inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
group by p.cuit
having sum(cmm.toneladas) between 5000 and 10000 )
order by p.cuit 


insert into #Valores (Cuit ,Toneladas ,Material,Campaña,Año,Segmentación,Provincia,Comercial,Mes,Criterio,razonSocial )
select cast(p.cuit as varchar(100)) as CUIT,cmm.toneladas as Toneladas,m.Descripcion as Material,c.Descripcion as Campaña,cast(cmm.Año as varchar(20)) as Año,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end,isnull(prv.Nombre,'SIN PROVINCIA' )as Provincia,emp.Apellido + ' ' + emp.Nombres as Comercial
,case when cmm.Mes=1 then 'ENERO'
when cmm.Mes=2 then 'FEBRERO' 
when cmm.Mes=3 then 'MARZO' 
when cmm.Mes=4 then 'ABRIL' 
when cmm.Mes=5 then 'MAYO' 
when cmm.Mes=6 then 'JUNIO' 
when cmm.Mes=7 then 'JULIO' 
when cmm.Mes=8 then 'AGOSTO' 
when cmm.Mes=9 then 'SEPTIEMBRE' 
when cmm.Mes=10 then 'OCTUBRE' 
when cmm.Mes=11 then 'NOVIEMBRE' 
when cmm.Mes=12 then 'DICIEMBRE' 
else 'SIN MES'
end  as Mes,
'Menos de 5000 TN.',
p.RazonSocial as razonSocial

from CampañaMaterialPorMes cmm
inner join CampañaMaterial cm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join proveedor p on p.ProveedorId= cm.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
left join Localidad loc on p.LocalidadId= loc.LocalidadId
left join provincia prv on loc.ProvinciaId = prv.ProvinciaId
inner join Segmentacion seg on seg.segmentacionId=p.segmentacionId
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
and p.cuit in (
select distinct  p.cuit
from proveedor p
inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
group by p.cuit
having sum(cmm.toneladas) < 5000 )
order by p.cuit 

select * from #Valores


drop table #Valores