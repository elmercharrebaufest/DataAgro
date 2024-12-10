
CREATE procedure [dbo].[DataAgro_Gauget_TraerExcel]

@comercialId Int= null,
@ComercialGenerador varchar(MAX) =null,
@CampañaId int = null,
@MaterialId int = null  

as

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);
   
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador
 
create table #Valor (MaterialId int,CampañaId int,CUIT float , Objetivo float, Compras float default(0),Porcentaje float default(0) )

insert into  #Valor (MaterialId,CUIT,Objetivo,CampañaId)
select  MaterialId,p.cuit, sum(ToneladasObjetivos) as Objetivo,CampañaId
from Objetivo o
inner join Proveedor p on o.ProveedorId = p.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ((@CampañaId is null) or (o.campañaId = @CampañaId))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
and ((@MaterialId is null) or (o.MaterialId = @MaterialId))
group by o.MaterialId,o.CampañaId ,p.cuit
having sum(ToneladasObjetivos) > 0


update #Valor
set Compras =  b.Toneladas,
Porcentaje= (b.Toneladas * 100) /objetivo
from (select cm.MaterialId as MaterialId,cm.campañaId as CampañaId,p.cuit , sum(cmm.Toneladas) as Toneladas
from CampañaMaterial cm
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId = cmm.CampañaMaterialId and cmm.ComercialId in ( select ComercialId from @EmpleadoTable)
inner join Proveedor p on cm.ProveedorId = p.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ((@CampañaId is null) or (cm.campañaId = @CampañaId))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
and ((@MaterialId is null) or (cm.MaterialId = @MaterialId))
group by cm.MaterialId,cm.campañaId,p.cuit) b
where #Valor.MaterialId = b.MaterialId and #Valor.CampañaId = b.CampañaId and #Valor.CUIT = b.CUIT and  b.Toneladas > 0


select p.cuit,val.Objetivo as Objetivos,round(val.Porcentaje,2) as Porcentajes
,isnull(cmm.toneladas,0) as Toneladas
,m.Descripcion as Material,
c.Descripcion as Campaña
,isnull(cast(cmm.Año as varchar(20)),'0') as Año
,isnull(prv.Nombre,'SIN PROVINCIA') as Provincia,emp.Apellido + ' ' + emp.Nombres as Comercial
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
end  as Mes
,case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,p.razonsocial
from #Valor val
inner join proveedor p on val.CUIT = p.CUIT
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
left join localidad loc on p.LocalidadId= loc.LocalidadId
left join Provincia prv on loc.ProvinciaId=prv.ProvinciaId
inner join Material m on val.MaterialId = m.MaterialId
inner join Campaña c on val.CampañaId = c.CampañaId
inner join segmentacion seg on seg.SegmentacionId = p.SegmentacionId
left join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId
left join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId  and cmm.comercialId in ( select ComercialId from @EmpleadoTable)

where 
((@CampañaId is null) or (val.campañaId = @CampañaId and cm.campañaId=@CampañaId  ))
and ((@MaterialId is null) or (val.MaterialId = @MaterialId and cm.MaterialId=@MaterialId ))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
and val.Porcentaje > cast(0 as float)

union all

select distinct p.cuit,val.Objetivo as Objetivos,val.Porcentaje as Porcentajes,
0 as Toneladas
,m.Descripcion as Material,
c.Descripcion as Campaña
,'0' as Año
,isnull(prv.Nombre,'SIN PROVINCIA') as Provincia,emp.Apellido + ' ' + emp.Nombres as Comercial
, 'SIN MES' as Mes
,case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,p.razonsocial
from #Valor val
inner join proveedor p on val.CUIT = p.CUIT
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
left join localidad loc on p.LocalidadId= loc.LocalidadId
left join Provincia prv on loc.ProvinciaId=prv.ProvinciaId
inner join Material m on val.MaterialId = m.MaterialId
inner join Campaña c on val.CampañaId = c.CampañaId
inner join segmentacion seg on seg.SegmentacionId = p.SegmentacionId
where 
((@CampañaId is null) or (val.campañaId = @CampañaId  ))
and ((@MaterialId is null) or (val.MaterialId = @MaterialId ))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
and val.Porcentaje = cast(0 as float) 
ORDER BY P.CUIT



drop table #Valor