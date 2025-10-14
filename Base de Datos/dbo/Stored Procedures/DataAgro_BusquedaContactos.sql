
CREATE procedure [dbo].[DataAgro_BusquedaContactos] 

@PeriodoDeTiempo varchar(max)= null,
@Segmentacion varchar(max) = null,
@Actividades varchar(max) = null,
@Materiales varchar(max) = null,
@Calificacion varchar(max) = null,
@Hectarea varchar(max) = null,
@Tonelada varchar(max) = null,
@comercialId varchar(max),
@Condicion varchar(max) = null,
@EstadoDelContacto varchar(max) = null,
@ComercialFiltro int,
@Zona int

AS

--set @PeriodoDeTiempo= '2|3'
--set @Segmentacion= '6|9'
--set @EstadoDelContacto= '2'
--set @Actividades= '1|3'
--set @Materiales= '1|3'
--set @Calificacion = '3'

--set @Hectarea = '1'
--set @Tonelada = '1|2'


declare @PeriodoDeTiempoSecuencia TABLE (Item INT)        
declare @SegmentacionSecuencia TABLE (Item INT)        
declare @EstadoDelContactoSecuencia TABLE (Item INT)        
declare @ActividadesSecuencia TABLE (Item INT)        
declare @MaterialesSecuencia TABLE (Item INT)        
declare @CalificacionSecuencia TABLE (Item INT)        
declare @Proveedores TABLE (Item INT) 
declare @HectareaSecuencia TABLE (Item INT) 
declare @ToneladaSecuencia TABLE (Item INT)        
declare @CondicionSecuencia TABLE (Item INT)
declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)
                  
insert into @PeriodoDeTiempoSecuencia (Item) select Item  from dbo.Split (@PeriodoDeTiempo,'|')        
insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@Segmentacion,'|')        
insert into @EstadoDelContactoSecuencia (Item) select Item  from dbo.Split (@EstadoDelContacto,'|')        
insert into @ActividadesSecuencia (Item) select Item  from dbo.Split (@Actividades,'|')      
insert into @MaterialesSecuencia (Item) select Item  from dbo.Split (@Materiales,'|')      
insert into @CalificacionSecuencia (Item) select Item  from dbo.Split (@Calificacion,'|');   
insert into @HectareaSecuencia (Item) select Item  from dbo.Split (@Hectarea,'|');   
insert into @ToneladaSecuencia (Item) select Item  from dbo.Split (@Tonelada,'|');      
insert into @CondicionSecuencia (Item) select Item from dbo.Split (@Condicion,'|');


insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @comercialId 


--TRAIGO LOS ID DE LOS PROVEEDORES QUE CUMPLEN LA CONDICIONES PEDIDAS
insert into @Proveedores(Item)
select distinct p.ProveedorId 
from Proveedor p
inner join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId
inner join @EmpleadoTable  e on e.ComercialId = pc.ComercialId
left join Acopio a on p.ProveedorId = a.ProveedorId
left join AcopioMaterial am on am.AcopioId = a.AcopioId
left join Campo ca on p.ProveedorId = ca.ProveedorId
left join CampoMaterial cam on cam.CampoId= ca.CampoId
left join CampanaMaterialDetallePorMes cm on cm.ProveedorId = p.ProveedorId
left join Actividad ac on p.ProveedorId = ac.ProveedorId
left join ProveedorCondicion pcc on pcc.ProveedorId = p.ProveedorId

where 1 = 1

and (( @PeriodoDeTiempo is null) or (@PeriodoDeTiempo= '0' and cam.CampañaId is not null) 
		or (exists ( select 1 from @PeriodoDeTiempoSecuencia where Item = cm.CampanaId)))	
		
and (( @Segmentacion is null) or (@Segmentacion= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and (( @Condicion is null) or (@Condicion= '0' and pcc.CondicionId is not null) 
	or (exists ( select 1 from @CondicionSecuencia where Item = pcc.CondicionId)))

and (( @Actividades is null)  or (@Actividades= '0' and ac.ActividadId is not null) 
	or (exists ( select 1 from @ActividadesSecuencia where Item = ac.TipoActividadId)))

and (( @Materiales is null) or (@Materiales= '0' and cam.CampoMaterialid is not null)
	or (exists ( select 1 from @MaterialesSecuencia where Item = cm.MaterialId)))
	
and (( @Calificacion is null) or (@Materiales= '0' and cam.CampoMaterialid is not null)
		or (exists ( select 1 from @CalificacionSecuencia where Item = p.Calificacion)))
		
and ((@Tonelada is null) or (@Tonelada ='0')
	or  (
			(exists(select 1 from @ToneladaSecuencia where Item = 1)) and (
				(
				( select SUM(camMat.Toneladas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )  
				+
				( select SUM(acoMat.Toneladas) from AcopioMaterial acoMat where acoMat.AcopioId = am.AcopioId )
				)
				between 1 and 2500 )
		)
	or  (
			(exists(select 1 from @ToneladaSecuencia where Item = 2)) and (
				( 
				( select SUM(camMat.Toneladas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )  
				+
				( select SUM(acoMat.Toneladas) from AcopioMaterial acoMat where acoMat.AcopioId = am.AcopioId )
				)
				between 2501 and 5000)
		)
	or  (
			(exists(select 1 from @ToneladaSecuencia where Item = 3)) and (
				(
				( select SUM(camMat.Toneladas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )    
				+
				( select SUM(acoMat.Toneladas) from AcopioMaterial acoMat where acoMat.AcopioId = am.AcopioId )
				)
				> 5000)
		)
	)  
and ((@Hectarea is null) 
	or (@Hectarea  ='0')
	or  (
			(exists(select 1 from @HectareaSecuencia where Item = 1)) and (( select SUM(camMat.Hectareas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )  between 1 and 500 )
		)
	or  (
			(exists(select 1 from @HectareaSecuencia where Item = 2)) and (( select SUM(camMat.Hectareas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )  between 501 and 1000)
		)
	or  (
			(exists(select 1 from @HectareaSecuencia where Item = 3)) and (( select SUM(camMat.Hectareas) from CampoMaterial camMat where camMat.CampoId = cam.CampoId )    > 1000)
		)
	)
and ((@ComercialFiltro is null) 
	or (@ComercialFiltro  ='0')
	or (e.ComercialId = @ComercialFiltro))
and ((@Zona is null) 
	or (@Zona  ='0')
	or (e.GrupoDeComprasId = @Zona))
	
DECLARE @ProveedorEstado TABLE(ProveedorId INT, EstadoId INT)
DECLARE @ProveedorEstadoHome TABLE(ProveedorId INT, EstadoId INT)

INSERT INTO @ProveedorEstado 
select 
	distinct t.Item , 
	case when exists(select 1 from ProveedorEstado pee where t.Item = pee.ProveedorId and  pee.EstadoId = 4 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
	then 4 else 
		case when exists(select 1 from ProveedorEstado pee where t.Item = pee.ProveedorId and  pee.EstadoId = 5 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
		then 5  else
				case when exists(select 1 from ProveedorEstado pee where t.Item = pee.ProveedorId and  pee.EstadoId = 1 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
				then 1  else
						case when exists(select 1 from ProveedorEstado pee where t.Item = pee.ProveedorId and  pee.EstadoId = 2 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
						then 2 else
							case when exists(select 1 from ProveedorEstado pee where t.Item = pee.ProveedorId and  pee.EstadoId = 3 and pee.ComercialId in (select ComercialId from  @EmpleadoTable))
							then 3  else
									(select EstadoId From Proveedor PP WHERE PP.ProveedorId = t.Item)
							ENd
						ENd
				ENd
		ENd
	ENd as Estado
	from @Proveedores t 
	LEFT join ProveedorEstado pe on t.Item = pe.ProveedorId and pe.ComercialId in (select ComercialId from  @EmpleadoTable)

INSERT INTO @ProveedorEstadoHome
SELECT DISTINCT t.Item, p.EstadoHomeId
FROM @Proveedores t 
LEFT JOIN Proveedor p ON t.Item=p.ProveedorId
LEFT JOIN EstadoHome eh ON p.EstadoHomeId=eh.Id

IF (@ComercialFiltro IS NOT NULL OR @ComercialFiltro!= '0' )
	DELETE @EmpleadoTable WHERE ComercialId != @ComercialFiltro

--DEVUELVO LOS RESULTADOS

SELECT 
	p.Calificacion, 
	p.SegmentacionId as Segmentacion,
	e.Apellido as ComercialAcargo,
	cc.Email1,
	cc.Email2,
	cc.Email3,
	null as Email4,
	p.CUIT,
	cc.Telefono1,
	cc.Telefono2,
	CC.Telefono3,
	null as Telefono4,
	p.RazonSocial,
	p.ProveedorId,
	p.FechaUltimoContacto,
	est.Descripcion as Estado ,
	CASE WHEN fc.CUIT is null then 0 else 1 end as Facacop,p.RiesgoComercialSap, isnull((select TOP 1 EstadoCuit from SISA where CUIT = p.CUIT),'') as EstadoCuit,
	p.EstadoHomeId, p.EstadoHomeMensaje, eh.Descripcion as EstadoHomeDescripcion, eh.Color, p.Cluster, p.Score, ISNULL( p.OperaConMATBA,0) as OperaConMATBA
FROM Proveedor p
left join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId
inner join @EmpleadoTable  e on e.ComercialId = pc.ComercialId
LEFT join @ProveedorEstado PEE ON PEE.ProveedorId = p.ProveedorId
LEFT join @ProveedorEstadoHome PEEH ON PEEH.ProveedorId = p.ProveedorId
LEFT join Estado est on est.EstadoId = pee.EstadoId
left join FACACOP fc on p.CUIT = fc.CUIT
/*left join RG2300 rg on p.CUIT = rg.CUIT*/
LEFT JOIN ContactoComercial CC ON CC.ProveedorId = p.ProveedorId AND CC.EsPrincipal = 1
LEFT JOIN EstadoHome eh ON p.EstadoHomeId = eh.Id
where exists (select 1 from @Proveedores where Item= p.ProveedorId)
and (( @EstadoDelContacto is null)
/*	or (exists ( select 1 from @EstadoDelContactoSecuencia where Item = pee.EstadoId)))*/
	or (exists ( select 1 from @EstadoDelContactoSecuencia where Item = PEEH.EstadoId)))