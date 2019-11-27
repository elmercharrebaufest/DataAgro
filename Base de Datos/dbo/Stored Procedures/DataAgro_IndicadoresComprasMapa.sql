CREATE PROCEDURE [dbo].[DataAgro_IndicadoresComprasMapa]  

@ProvinciaId int,
@SegmentacionId VARCHAR(max),

@MaterialId int,
@CampañaId int,

@ComercialId int =null,
@ComercialGenerador VARCHAR(max)=null 

as

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);
declare @SegmentacionSecuencia TABLE (Item INT)    
declare @ProvinciaSecuencia TABLE (Item INT) 

if (@ProvinciaId is not null)
begin 
	if (@ProvinciaId = 0 or @ProvinciaId = 1)
	begin 
	   insert into @ProvinciaSecuencia (item) values(0)
	   insert into @ProvinciaSecuencia (item) values(1)
	end
	else
		insert into @ProvinciaSecuencia (item) values(@ProvinciaId)

end

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador
 
create table #Valores(Prov varchar(200),Cl bigint,Tn Float)

insert into #Valores(Prov,Cl)

select distinct prv.Nombre, p.cuit

from proveedor p

inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId

inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId

inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId

inner join Localidad loc on p.LocalidadId= loc.LocalidadId

inner join provincia prv on loc.ProvinciaId = prv.ProvinciaId

where ((@MaterialId is null) or (cm.MaterialId= @MaterialId))

and ((@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and (( @ProvinciaId is null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.provinciaId)))


and p.LocalidadId is not null

 

 

update #Valores
set Tn= b.tn
from
(select  p.cuit,prv.Nombre as Provincia, sum (cmm.toneladas) as Tn 
from proveedor p
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join CampañaMaterial cm on p.ProveedorId= cm.ProveedorId

inner join CampañaMaterialPorMes cmm on cm.CampañaMaterialId=cmm.CampañaMaterialId

inner join Localidad loc on p.LocalidadId= loc.LocalidadId

inner join provincia prv on loc.ProvinciaId = prv.ProvinciaId

where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))

and ((@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and ((@SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

--and ((@ProvinciaId is null) or (p.provinciaId = @ProvinciaId))
and (( @ProvinciaId is null) --or (@ProvinciaId= '0' and p.provinciaId is not null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.provinciaId)))

and p.LocalidadId is not null

group by p.cuit,prv.Nombre ) b

where #Valores.Cl= b.CUIT  and b.Provincia= #Valores.Prov

 

select   max(Prov)  as Provincia, sum(tn) as Tonelada,count(CL) as Cuit

from #Valores

group by prov

 

drop table #Valores