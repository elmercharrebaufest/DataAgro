CREATE  procedure [dbo].[DataAgro_Gauget_Traer]

@comercialId INT=null,
@ComercialGenerador VARCHAR(max)=null,
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
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ((@CampañaId is null) or (o.CampañaId = @CampañaId))
and ((@MaterialId is null) or (o.MaterialId = @MaterialId))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
group by o.MaterialId,o.CampañaId ,p.cuit
having sum(ToneladasObjetivos) > 0

update #Valor
set Compras =  b.Toneladas,
Porcentaje= (b.Toneladas * 100) /objetivo
from (select cm.MaterialId as MaterialId,cm.CampañaId as CampañaId,p.cuit , sum(cmm.Toneladas) as Toneladas
from CampañaMaterial cm
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId = cmm.CampañaMaterialId and cmm.comercialId in ( select ComercialId from @EmpleadoTable)
inner join Proveedor p on cm.ProveedorId = p.ProveedorId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ((@CampañaId is null) or (cm.CampañaId = @CampañaId))
and ((@MaterialId is null) or (cm.MaterialId = @MaterialId))
and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))
group by cm.MaterialId,cm.CampañaId,p.cuit) b
where #Valor.MaterialId = b.MaterialId and #Valor.CampañaId = b.CampañaId and #Valor.CUIT = b.CUIT and  b.Toneladas > 0

select m.Descripcion as Material , cast(cast(sum(Porcentaje) / count(Porcentaje) as decimal(18,2)) as float) as Porcentajes  
from #Valor v
inner join Material m on m.MaterialId=v.MaterialId
where v.Objetivo > 0
group by m.Descripcion

select * from #Valor

drop table #Valor
