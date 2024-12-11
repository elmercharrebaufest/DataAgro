CREATE Procedure [dbo].[DataAgro_Proveedores_Cubo]
    @ProveedorId int = null,
    @ProvinciaId int = null,
    @LocalidadId int = null,
    @EstadoId int = null,
    @SegmentacionId int = null,
    @Material int = null,
    @ComercialId int 
as

--RECURSIVIDAD
WITH Empleados 
(ComercialId)
AS
(
    SELECT ComercialId
    FROM Comercial  
    WHERE ComercialId = @ComercialId 
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
        case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 4 and pee.ComercialId in (select ComercialId from  #Empleados)) 
        then 4 else 
            case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 5 and pee.ComercialId in (select ComercialId from  #Empleados))  
            then 5  else
                    case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 1 and pee.ComercialId in (select ComercialId from  #Empleados))  
                    then 1  else
                            case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 2 and pee.ComercialId in (select ComercialId from  #Empleados))  
                            then 2 else
                                case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 3 and pee.ComercialId in (select ComercialId from  #Empleados))  
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
matCamp.Descripcion as MaterialCampaña,
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
from Proveedor p
inner join [ProveedorComercial] pc on p.ProveedorId  = pc.ProveedorId
inner join #Empleados emp on pc.ComercialId = emp.ComercialId
left join [Localidad] l on p.LocalidadId = l.LocalidadId
left join [Provincia] pr on pr.ProvinciaId = l.ProvinciaId
inner join [Segmentacion] s on s.SegmentacionId = p.SegmentacionId
left join [ContactoComercial] cc on cc.ProveedorId =p.ProveedorId
left join @ProveedorEstado PEE ON PEE.ProveedorId = p.ProveedorId
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
left join [Localidad] lca on ca.LocalidadId = lca.LocalidadId
left join [Provincia] prca on prca.ProvinciaId = lca.ProvinciaId
left join [CampoMaterial] caMat on caMat.[CampoId] = ca.[CampoId]
left join [Material] matCa on caMat.[MaterialId] = matCa.[MaterialId]
left join [Campaña] CCa on caMat.[CampañaId] = CCa.[CampañaId]
--Acopio
left join Acopio ac on ac.[ProveedorId] = p.[ProveedorId] 
left join [Localidad] lcac on ac.LocalidadId = lcac.LocalidadId
left join [Provincia] prac on prac.ProvinciaId = lcac.ProvinciaId
left join AcopioMaterial acMat on acMat.AcopioId = ac.AcopioId
left join [Campaña] Cac on acMat.[CampañaId] = Cac.[CampañaId]
where 1=1
--cc.[EsPrincipal] = 1
and ((@EstadoId is null) or (p.EstadoId =@EstadoId))
and ((@SegmentacionId is null) or (p.SegmentacionId =@SegmentacionId))
--Provincia y localidad
and ((@ProvinciaId is null) or (prca.ProvinciaId =@ProvinciaId))
and ((@LocalidadId is null) or (ca.LocalidadId =@LocalidadId))
and ((@ProvinciaId is null) or (prac.ProvinciaId =@ProvinciaId))
and ((@LocalidadId is null) or (ac.LocalidadId =@LocalidadId))
--Material
and ((@Material is null) or (cm.MaterialId =@Material))
and ((@Material is null) or (caMat.MaterialId =@Material))
--Proveedor
and ((@ProveedorId is null) or (p.ProveedorId =@ProveedorId)) 




if((select count(CUIT) from #Prueba) > 1)
begin 
    select ROW_NUMBER() over (partition by p.CUIT order by  p.CUIT asc) as Id, p.CUIT, p.CampañaCampo,MaterialCampo,
    LocalidadCampo,ProvinciaCampo
    INTO #CANTIDAD
    from #Prueba p
    group by p.CUIT,p.CampañaCampo,MaterialCampo,LocalidadCampo,ProvinciaCampo

    select CUIT, MIN(Id) AS ID
    INTO #CANTIDADPORCUIT
    from #CANTIDAD
    group by CUIT

    select p.CUIT, count(p.CampañaCampo) AS CantidadDeRepeticiones
    into #Divisiones
    from #Prueba p,
    ( select C.CUIT, C.CampañaCampo,C.MaterialCampo,C.LocalidadCampo,C.ProvinciaCampo 
    from #CANTIDAD  C
    INNER JOIN  #CANTIDADPORCUIT D ON C.CUIT = D.CUIT AND C.Id=D.ID	) c
    where p.CampañaCampo =c.CampañaCampo and p.MaterialCampo = c.MaterialCampo 
    and p.LocalidadCampo = c.LocalidadCampo and p.ProvinciaCampo = c.ProvinciaCampo AND P.CUIT = C.CUIT
    group by p.CUIT

    select CUIT,MaterialCampaña,Campaña, count(MaterialCampaña) as CantidadCom
    into #CantidadCompradas
    from #Prueba p
    group by CUIT,MaterialCampaña,Campaña

    --select * from #CantidadCompradas

    update #Prueba 
    set HectCampo = (cast(HectCampo as float)/ d.CantidadDeRepeticiones),
    TonCampo = (cast(TonCampo as float)/ d.CantidadDeRepeticiones),
    TonAcopio = (cast(TonAcopio as float)/ d.CantidadDeRepeticiones)
    from #Divisiones d
    where d.CUIT = #Prueba.CUIT

    update #Prueba 
    set ToneladasCompradas = (cast(ToneladasCompradas as float)/ d.CantidadCom),
    ToneladasObjetivo = (cast(ToneladasObjetivo as float)/ d.CantidadCom)
    from #CantidadCompradas d
    where d.CUIT = #Prueba.CUIT and d.MaterialCampaña = #Prueba.MaterialCampaña and d.Campaña  = #Prueba.Campaña 


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
    left join #CantidadCompradas cc on p.CUIT = cc.CUIT and cc.MaterialCampaña = p.MaterialCampaña and cc.Campaña  = p.Campaña 
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
