CREATE  PROCEDURE [dbo].[DataAgro_IndicadoresBaseDeDatos_Traer]  

	@fechaDesde datetime=null,
	@fechaHasta datetime=null,
	@Mes int=null,
	@SegmentacionId VARCHAR(max) ,
	@CampañaId int = null,
	@Toneladas float = null,
	@ComercialId int = null,
	@ComercialGenerador VARCHAR(max)=null,
	@MaterialId int = null

as 
declare @Proveedores TABLE (Item INT) 
declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);

declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador
 
insert into @Proveedores(Item)
select distinct p.ProveedorId 
from Proveedor p
inner join ProveedorComercial pc on pc.ProveedorId= p.ProveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Segmentacion s on  p.SegmentacionId = s.SegmentacionId
left join Campo ca on p.ProveedorId = ca.ProveedorId
left join CampoMaterial cam on cam.CampoId= ca.CampoId
where cast(p.FechaAlta as date) between @fechaDesde and @fechaHasta

and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))

and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and ((@Toneladas is null) or (@Toneladas ='0')
		or  ( (@Toneladas = 1) and ( ( ( select SUM(camMat.Toneladas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )  between 1 and 2500 ) ))
	
		or  ( (@Toneladas = 2) and ( (  ( select SUM(camMat.Toneladas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId ) between 2501 and 5000) ))
	
		or  ( (@Toneladas = 3) and ( ( 	( select SUM(camMat.Toneladas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )   > 5000) ))	
	)

and (( @MaterialId is null) or ( cam.MaterialId= @MaterialId  ))

and (( @Mes is null) or ( MONTH (p.FechaAlta) = @Mes  ))

/*and ((@ComercialId is null) 
	or (@ComercialId  ='0')
	or (emp.ComercialId = @ComercialId))*/

DECLARE @ProveedorEstado TABLE(ProveedorId INT, EstadoId INT)

INSERT INTO @ProveedorEstado 
select 
distinct t.item , 
case when exists(select 1 from ProveedorEstado pee where t.item = pee.ProveedorId and  pee.EstadoId = 4 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
then 4 else 
	case when exists(select 1 from ProveedorEstado pee where t.item = pee.ProveedorId and  pee.EstadoId = 5 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
	then 5  else
			case when exists(select 1 from ProveedorEstado pee where t.item = pee.ProveedorId and  pee.EstadoId = 1 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
			then 1  else
					case when exists(select 1 from ProveedorEstado pee where t.item = pee.ProveedorId and  pee.EstadoId = 2 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
					then 2 else
						case when exists(select 1 from ProveedorEstado pee where t.item = pee.ProveedorId and  pee.EstadoId = 3 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
						then 3  else
								(select EstadoId From Proveedor PP WHERE PP.ProveedorId = t.item)
						ENd
					ENd
			ENd
	ENd
ENd as Estado
from @Proveedores t 
LEFT join ProveedorEstado pe on t.item = pe.ProveedorId and pe.ComercialId in (select ComercialId from  @EmpleadoTable)


select 
p.cuit as Cuit,
p.razonsocial as RazonSocial,
est.descripcion as Estado,
case when  s.grupo ='Productores' then 'Productores ' + s.Descripcion   else s.Descripcion end as Segmentación,
p.FechaAlta as FechaAlta, 
e.Apellido + ' ' + e.Nombres as Comercial,
isnull(M.Descripcion,'Sin Material') as Grano, 
isnull(SUM(CP.Toneladas),0) as Toneladas
FROM @ProveedorEstado PEE
inner join Proveedor p ON PEE.ProveedorId = p.ProveedorId
inner join Segmentacion s on  p.SegmentacionId = s.SegmentacionId
left join Campo C on C.ProveedorId = PEE.ProveedorId
left join CampoMaterial CP on CP.CampoId = C.CampoId --and (@MaterialId is null or cp.MaterialId = @MaterialId) and (@CampañaId is null or cp.CampañaId = @CampañaId) 
left JOIN Material M ON M.MaterialId = CP.MaterialId
left join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId
inner join @EmpleadoTable  e on e.ComercialId = pc.ComercialId
LEFT join Estado est on est.EstadoId = pee.EstadoId
where 
( @MaterialId is null or exists (select 1 from CampoMaterial cp2 where cp2.MaterialId = @MaterialId and cp.CampoMaterialid = cp2.CampoMaterialid) )

and ( @CampañaId is null or exists (select 1 from CampoMaterial cp3 where cp3.CampañaId = @CampañaId and cp.CampoMaterialid = cp3.CampoMaterialid) )

and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))

Group by p.cuit, p.razonsocial, est.descripcion, s.descripcion,s.grupo, p.FechaAlta, e.Apellido + ' ' + e.Nombres, M.Descripcion
