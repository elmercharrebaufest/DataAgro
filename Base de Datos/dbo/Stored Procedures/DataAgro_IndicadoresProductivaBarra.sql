CREATE PROCEDURE [dbo].[DataAgro_IndicadoresProductivaBarra]  

@SegmentacionId VARCHAR(max),
@MaterialId int,
@CampañaId int,   
@ComercialId int =null,
@ComercialGenerador VARCHAR(max)=null

as

declare 
@barra1 int = 5000,
@barra2 int = 10000,
@barra3 int = 20000 
 
declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);
   
declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;


insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador

create table #Valores(MasTn20000 float, MasCl20000 int, MasTn10000 float, MasCl10000 int, MasTn1000 float,MasCl1000 int,MasTn100 float,MasCl100 int)

insert into #Valores (MasCl20000, MasTn20000)
select count(b.Cuit),sum(b.Toneladas)
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Campo cm on p.ProveedorId= cm.ProveedorId
inner join CampoMaterial cmm on cm.CampoId=cmm.CampoId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))
group by p.cuit
having sum(cmm.toneladas) > @barra3 )b

update #Valores
set MasTn10000= c.toneladas,
	MasCl10000 = c.cantidad
from (select count(b.Cuit) as cantidad,sum(b.Toneladas) as toneladas
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Campo cm on p.ProveedorId= cm.ProveedorId
inner join CampoMaterial cmm on cm.CampoId=cmm.CampoId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))

group by p.cuit
having sum(cmm.toneladas) between @barra2 and @barra3 - 1 )b) c


update #Valores
set MasTn1000= c.toneladas,
	MasCl1000 = c.cantidad
from (select count(b.Cuit) as cantidad,sum(b.Toneladas) as toneladas
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Campo cm on p.ProveedorId= cm.ProveedorId
inner join CampoMaterial cmm on cm.CampoId=cmm.CampoId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))
 
group by p.cuit
having sum(cmm.toneladas) between @barra1 and @barra2 - 1 )b) c


update #Valores
set MasTn100= c.toneladas,
	MasCl100 = c.cantidad
from (select count(b.Cuit) as cantidad,sum(b.Toneladas) as toneladas
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Campo cm on p.ProveedorId= cm.ProveedorId
inner join CampoMaterial cmm on cm.CampoId=cmm.CampoId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))
group by p.cuit
having sum(cmm.toneladas) < @barra1)b) c



select * from #Valores

drop table #Valores