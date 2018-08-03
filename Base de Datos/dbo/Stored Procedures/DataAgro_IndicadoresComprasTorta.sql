CREATE PROCEDURE [dbo].[DataAgro_IndicadoresComprasTorta]  

@MaterialId int   ,

@CampañaId int    ,

@ComercialId int =null,
@ComercialGenerador Int=null   

as
declare @ProveedoresTable TABLE ( ProveedorId int , Segmentacion varchar(255))
declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);
   


insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador


insert into @ProveedoresTable(ProveedorId)
select distinct p.ProveedorId
from CampañaMaterialPorMes cmm
inner join CampañaMaterial cm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join proveedor p on p.ProveedorId= cm.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId

where((@MaterialId is null) or (CM.MaterialId=@MaterialId ))
and ((@CampañaId is null) or (cm.CampañaId = @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))


select  seg.Descripcion as Provincia,count(p.proveedorId)as cuit,0 as Tonelada
into #Valor
from @ProveedoresTable pt
inner join proveedor p on p.proveedorId = pt.proveedorId
inner join segmentacion seg on p.SegmentacionId=seg.SegmentacionId
group by seg.Descripcion


select seg.Descripcion as Segmentacion, sum(cmm.toneladas)as Tonelada
into #Tonelada
from CampañaMaterialPorMes cmm
inner join CampañaMaterial cm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join proveedor p on p.ProveedorId= cm.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId
inner join segmentacion seg on p.SegmentacionId=seg.SegmentacionId
where   ((@MaterialId is null) or (CM.MaterialId=@MaterialId ))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and ( (@CampañaId is null) or (cm.CampañaId = @CampañaId)) 
group by seg.Descripcion


select case when seg.grupo ='Productores' then 'Productores ' + v.Provincia   else v.Provincia end Provincia,   v.cuit, ton.Tonelada
from #Valor v
inner join segmentacion seg on v.Provincia=seg.Descripcion
inner join #Tonelada ton on ton.Segmentacion= v.Provincia
order by ton.Tonelada desc

drop table #Valor