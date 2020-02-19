
CREATE Procedure [dbo].[DataAgro_Proveedores_Cubo]
    @proveedorId int = null,
	@provinciaId int = null,
	@LocalidadId int = null,
	@EstadoId int = null,
	@SegmentacionId int = null,
	@material int = null,
	@ComercialId int 
as

--RECURSIVIDAD
WITH Empleados 
( ComercialId)
AS
(
	SELECT ComercialId
    FROM Comercial  
	WHERE ComercialId = @comercialId 
	UNION ALL 
	SELECT A.ComercialId
	FROM Comercial A
	inner join Empleados AS B on A.EmpleadorACargoId = B.ComercialId
)

select * 
into #Empleados
from Empleados

 

SELECT 
	p.Calificacion, 
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
	p.EstadoId,
	est.Descripcion as Estado ,
	CASE WHEN fc.CUIT is null then 0 else 1 end as Facacop,p.RiesgoComercialSap, isnull((select TOP 1 EstadoCuit from SISA where CUIT = p.CUIT),'') as situacion 
into #ProveedorAux
FROM Proveedor p  
LEFT join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId 
INNER join #Empleados e on e.ComercialId = pc.ComercialId
LEFT join Estado est on p.EstadoId = est.EstadoId
LEFT join FACACOP fc on p.CUIT = fc.CUIT
LEFT JOIN ContactoComercial CC ON CC.ProveedorId = p.ProveedorId AND CC.EsPrincipal = 1
    



	DECLARE @ProveedorEstado TABLE(ProveedorId INT, EstadoId INT)

	INSERT INTO @ProveedorEstado 
	select 
		distinct t.ProveedorId , 
		case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 4 and pee.ComercialId in (select ComercialId from  #Empleados)) 
		then 4 else 
			case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 5 and pee.ComercialId in (select ComercialId from  #Empleados))  
			then 5  else
					case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 1 and pee.ComercialId in (select ComercialId from  #Empleados))  
					then 1  else
							case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 2 and pee.ComercialId in (select ComercialId from  #Empleados))  
							then 2 else
								case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 3 and pee.ComercialId in (select ComercialId from  #Empleados))  
								then 3  else
										(select EstadoId From Proveedor PP WHERE PP.ProveedorId = t.ProveedorId)
								ENd
							ENd
					ENd
			ENd
		ENd as Estado
		from #ProveedorAux t 
	LEFT join ProveedorEstado pe on t.ProveedorId = pe.ProveedorId and pe.ComercialId in (select ComercialId from  #Empleados)
	--where pe.ComercialId in (select ComercialId from  #Empleados)
	--group by pe.ProveedorId




select p.CUIT,
p.RazonSocial,
s.Descripcion as Sergmentacion,
e.Descripcion as Estado,
cc.Apellido + ' ' + cc.Nombres  as ContactoApellido,
a.[Descripcion] as AreaDeInfluencia,
-- Campos Nuevos
matCamp.descripcion as MaterialCampaña,
c.[Descripcion] as  Campaña,
cast(isnull((
select Sum(cmm.Toneladas) 
from CampañaMaterialPorMes cmm 
where CampañaMaterialId = cm.CampañaMaterialId and emp.ComercialId = cmm.ComercialId
)
,0) as float) as ToneladasCompradas ,
/*0 as ToneladasObjetivo,*/cast(isnull(obj.[ToneladasObjetivos],0) as float) as ToneladasObjetivo,
CCa.[Descripcion] as CampañaCampo,
matCa.Descripcion as MaterialCampo,
cast(isnull(caMat.[Hectareas],0)  as float) as HectCampo,
cast(isnull(caMat.[Toneladas],0)  as float) as TonCampo,
lca.[Nombre] as LocalidadCampo,
prca.[Nombre] as ProvinciaCampo,
Cac.[Descripcion] as CampañaAcopio,
cast(isnull(acMat.[Toneladas],0)  as float) as TonAcopio,
lca.[Nombre] as LocalidadAcopio,
prca.[Nombre] as ProvinciaAcopio
into #Prueba
from proveedor p
inner join [ProveedorComercial] pc on p.ProveedorId  = pc.ProveedorId
inner join #Empleados emp on pc.ComercialId = emp.ComercialId
left join [Localidad] l on p.localidadId = l.localidadId
left join [Provincia] pr on pr.provinciaid = l.provinciaid
inner join [Segmentacion] s on s.segmentacionId = p.segmentacionId
left join [ContactoComercial] cc on cc.ProveedorId =p.ProveedorId
left join @ProveedorEstado PEE ON PEE.ProveedorID = p.ProveedorId
inner join [Estado] e on e.[EstadoId] = PEE.EstadoId
left join [AreaInfluencia] a on a.[AreaInfluenciaId] = p.[AreaInfluenciaId]
--Campaña
left join CampañaMaterial cm on p.ProveedorId = cm.[ProveedorId]
left join Campaña c on cm.[CampañaId]=c.[CampañaId]
left join Material matCamp on cm.MaterialId = matCamp.[MaterialId]
--left join CampañaMaterialPorMes cmm on cmm.CampañaMaterialId = cm.CampañaMaterialId


--Objetivos
left join Objetivo obj on p.ProveedorId = obj.[ProveedorId] and cm.MaterialId = obj.MaterialId and cm.CampañaId = obj.CampañaId
/*left join Campaña cobj on cm.[CampañaId]=c.[CampañaId] 
left join Material matObj on cm.MaterialId = matCamp.[MaterialId]*/


--Campos
left join [Campo] ca on ca.[ProveedorId] = p.[ProveedorId] 
left join [Localidad] lca on ca.localidadId = lca.localidadId
left join [Provincia] prca on prca.provinciaid = lca.provinciaid
left join [CampoMaterial] caMat on caMat.[CampoId] = ca.[CampoId]
left join [Material] matCa on caMat.[MaterialId] = matCa.[MaterialId]
left join [Campaña] CCa on caMat.[CampañaId] = CCa.[CampañaId]
--Acopio
left join Acopio ac on ac.[ProveedorId] = p.[ProveedorId] 
left join [Localidad] lcac on ac.localidadId = lcac.localidadId
left join [Provincia] prac on prac.provinciaid = lcac.provinciaid
left join AcopioMaterial acMat on acMat.AcopioId = ac.AcopioId
left join [Campaña] Cac on acMat.[CampañaId] = Cac.[CampañaId]
where 1=1
--cc.[EsPrincipal] = 1
and ((@EstadoId is null) or (p.EstadoId =@EstadoId))
and ((@SegmentacionId is null) or (p.SegmentacionId =@SegmentacionId))
--Provincia y localidad
and ((@provinciaId is null) or (prca.provinciaid =@provinciaId))
and ((@LocalidadId is null) or (ca.localidadId =@LocalidadId))
and ((@provinciaId is null) or (prac.provinciaid =@provinciaId))
and ((@LocalidadId is null) or (ac.localidadId =@LocalidadId))
--Material
and ((@material is null) or (cm.MaterialId =@material))
and ((@material is null) or (caMat.MaterialId =@material))
--Proveedor
and ((@proveedorId is null) or (p.ProveedorId =@proveedorId)) 




if((select count(CUIT) from #Prueba) > 1)
begin 
	select ROW_NUMBER() over (partition by p.cuit order by  p.cuit asc) as Id, p.cuit, p.CampañaCampo,MaterialCampo,
	LocalidadCampo,ProvinciaCampo
	INTO #CANTIDAD
	from #Prueba p
	group by p.cuit,p.CampañaCampo,MaterialCampo,LocalidadCampo,ProvinciaCampo

	select cuit, MIN(ID) AS ID
	INTO #CANTIDADPORCUIT
	from #CANTIDAD
	group by cuit

	select p.cuit, count(p.CampañaCampo) AS CantidadDeRepeticiones
	into #Divisiones
	from #Prueba p,
	( select C.CUIT, C.CampañaCampo,C.MaterialCampo,C.LocalidadCampo,C.ProvinciaCampo 
	from #CANTIDAD  C
	INNER JOIN  #CANTIDADPORCUIT D ON C.CUIT = D.CUIT AND C.Id=D.ID	) c
	where p.CampañaCampo =c.CampañaCampo and p.MaterialCampo = c.MaterialCampo 
	and p.LocalidadCampo = c.LocalidadCampo and p.ProvinciaCampo = c.ProvinciaCampo AND P.CUIT = C.CUIT
	group by p.cuit

	select cuit,MaterialCampaña,Campaña, count(MaterialCampaña) as CantidadCom
	into #CantidadCompradas
	from #Prueba p
	group by cuit,MaterialCampaña,Campaña

	--select * from #CantidadCompradas

	update #Prueba 
	set HectCampo = (cast(HectCampo as float)/ d.CantidadDeRepeticiones),
	TonCampo = (cast(TonCampo as float)/ d.CantidadDeRepeticiones),
	TonAcopio = (cast(TonAcopio as float)/ d.CantidadDeRepeticiones)
	from #Divisiones d
	where d.CUIT = #Prueba.cuit

	update #Prueba 
	set ToneladasCompradas = (cast(ToneladasCompradas as float)/ d.CantidadCom),
	ToneladasObjetivo = (cast(ToneladasObjetivo as float)/ d.CantidadCom)
	from #CantidadCompradas d
	where d.CUIT = #Prueba.cuit and d.MaterialCampaña = #Prueba.MaterialCampaña and d.Campaña  = #Prueba.Campaña 


	SELECT  p.CUIT,
		RazonSocial,
		Sergmentacion,
		Estado,
		ContactoApellido,
		AreaDeInfluencia,
		p.MaterialCampaña,
		p.Campaña,
		ToneladasCompradas,
		ToneladasObjetivo,
		CampañaCampo,
		MaterialCampo,
		HectCampo,
		TonCampo,
		LocalidadCampo,
		ProvinciaCampo,
		CampañaAcopio,
		TonAcopio,
		LocalidadAcopio,
		ProvinciaAcopio
	FROM #Prueba p
	--left join #Divisiones d on p.CUIT = d.CUIT
	left join #CantidadCompradas cc on p.cuit = cc.cuit and cc.MaterialCampaña = p.MaterialCampaña and cc.Campaña  = p.Campaña 
	ORDER BY p.CUIT

	DROP TABLE #Divisiones
	DROP TABLE #CantidadCompradas
	DROP TABLE #CANTIDAD

	DROP TABLE  #CANTIDADPORCUIT

END 
ELSE 
BEGIN 

	SELECT * FROM #Prueba

END

DROP TABLE #Prueba
DROP TABLE #Empleados
