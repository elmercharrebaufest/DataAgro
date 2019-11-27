CREATE PROCEDURE [dbo].[DataAgro_IndicadoresAcopioBarra]  

 @SegmentacionId VARCHAR(max) ,
 @MaterialId int ,
 @CampañaId int , 
 @ComercialId int =null,
 @ComercialGenerador VARCHAR(max)=null

 as

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);
  
declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;  
   
--RECURSIVIDAD
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador

--create table #Valores(MasTn5000 float,MasCl5000 int,MasTn1000 float,MasCl1000 int,MasTn100 float,MasCl100 int)


create table #Valores(MasTn5000 float,MasCl5000 int,MasTn1000 float,MasCl1000 int,MasTn100 float,
					  MasCl100 int, MenosTn10000 float, MenosCl10000 int, MasTn10000 float, MasCl10000 int,
					  MasTn20000 float, MasCl20000 int, MasTn40000 float, MasCl40000 int)


insert into #Valores (MenosCl10000,MenosTn10000)
select c.cantidad,c.toneladas
from (select count(b.Cuit) as cantidad,sum(b.Toneladas) as toneladas
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))
group by p.cuit
having sum(cmm.toneladas) < 10000)b) c


update #Valores
set MasTn10000= c.toneladas,
	MasCl10000 = c.cantidad
from (select count(b.Cuit) as cantidad,sum(b.Toneladas) as toneladas
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))
group by p.cuit
having sum(cmm.toneladas) between 10000 and 20000 )b) c





update #Valores
set MasTn20000= c.toneladas,
	MasCl20000 = c.cantidad
from (select count(b.Cuit) as cantidad,sum(b.Toneladas) as toneladas
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))
group by p.cuit
having sum(cmm.toneladas) between 20000 and 40000 )b) c



update #Valores
set MasTn40000= c.toneladas,
	MasCl40000 = c.cantidad
from (select count(b.Cuit) as cantidad,sum(b.Toneladas) as toneladas
from (select p.cuit,sum(cmm.Toneladas) as Toneladas
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Acopio cm on p.ProveedorId= cm.ProveedorId
inner join AcopioMaterial cmm on cm.AcopioId=cmm.AcopioId
where ( (@CampañaId is null) or (cmm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and((@MaterialId is null) or (cmm.MaterialId = @MaterialId))
group by p.cuit
having sum(cmm.toneladas) > 40000 )b) c



select * from #Valores

drop table #Valores