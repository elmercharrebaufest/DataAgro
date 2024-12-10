
CREATE procedure [dbo].[Reporte_ComprasBarra_Traer]
 
 @Mes int= null,
 @SegmentacionId VARCHAR(max) ,
 @MaterialId int= null,
 @CampañaId int= null,
 @comercialId int =null,
 @ComercialGenerador VARCHAR(max)=null  
as

 
declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeCompras int);
   
declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',');


insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador

create table #Valores(MasTn5000 float,MasCl5000 int,MasTn1000 float,MasCl1000 int,MasTn100 float,MasCl100 int)

insert into #Valores (MasCl5000,MasTn5000)
select count(b.CUIT),sum(b.Toneladas)
from (select p.CUIT,sum(cmm.Toneladas) as Toneladas
from Proveedor p
inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and  ( (@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
group by p.CUIT
having sum(cmm.toneladas) > 10000 )b


update #Valores
set MasTn1000= c.toneladas,
	MasCl1000 = c.cantidad
from (select count(b.CUIT) as cantidad,sum(b.Toneladas) as toneladas
from (select p.CUIT,sum(cmm.Toneladas) as Toneladas
from Proveedor p
inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and  ( (@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
group by p.CUIT
having sum(cmm.toneladas) between 5000 and 10000 )b) c


update #Valores
set MasTn100= c.toneladas,
	MasCl100 = c.cantidad
from (select count(b.CUIT) as cantidad,sum(b.Toneladas) as toneladas
from (select p.CUIT,sum(cmm.Toneladas) as Toneladas
from Proveedor p
inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId
inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
where ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@MaterialId is null) or (cm.MaterialId= @MaterialId))
and  ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ( (@Mes is null) or (cmm.Mes = @Mes))
group by p.CUIT
having sum(cmm.toneladas) < 5000 )b) c



select * from #Valores

drop table #Valores