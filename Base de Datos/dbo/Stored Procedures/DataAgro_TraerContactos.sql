
CREATE PROCEDURE [dbo].[DataAgro_TraerContactos] 

 @comercialId int
 
 AS

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)
 
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @comercialId 

--select * 
--	into #Empleados
--	from Empleados

	SELECT 
		p.Calificacion, 
		e.Apellido as ComercialAcargo,
		cc.Email1,
		cc.Email2,
		cc.Email3,
		null as Email4,
		p.CUIT,
		cc.Telefono1,
		cc.Telefono2,
		cc.Telefono3,
		null as Telefono4,
		p.RazonSocial,
		p.ProveedorId,
		p.FechaUltimoContacto,
		p.EstadoId,
		est.Descripcion as Estado ,
		CASE WHEN fc.CUIT is null then 0 else 1 end as Facacop,p.RiesgoComercialSap, isnull((select TOP 1 Situacion from rg2300 where CUIT = p.CUIT),'') as situacion 
	into #ProveedorAux
	FROM Proveedor p  
	LEFT join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId 
	LEFT JOIN ContactoComercial CC ON CC.ProveedorId = p.ProveedorId AND CC.EsPrincipal = 1
	INNER join @EmpleadoTable e on e.ComercialId = pc.ComercialId
	LEFT join Estado est on p.EstadoId = est.EstadoId
	LEFT join FACACOP fc on p.CUIT = fc.CUIT
    



	DECLARE @ProveedorEstado TABLE(ProveedorId INT, EstadoId INT)

	INSERT INTO @ProveedorEstado 
	select 
		distinct t.ProveedorId , 
		case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 4 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
		then 4 else 
			case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 5 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
			then 5  else
					case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 1 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
					then 1  else
							case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 2 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
							then 2 else
								case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 3 and pee.ComercialId in (select ComercialId from  @EmpleadoTable)) 
								then 3  else
								1
										--(select EstadoId From Proveedor PP WHERE PP.ProveedorId = t.ProveedorId)
								ENd
							ENd
					ENd
			ENd
		ENd as Estado
		from #ProveedorAux t 
	LEFT join ProveedorEstado pe on t.ProveedorId = pe.ProveedorId and pe.ComercialId in (select ComercialId from  @EmpleadoTable)
	--where pe.ComercialId in (select ComercialId from  #Empleados)
	--group by pe.ProveedorId

SELECT 
	p.Calificacion, 
	e.Apellido as ComercialAcargo,
	cc.Email1,
	cc.Email2,
	cc.Email3,
	null as Email4,
	p.CUIT,
	cc.Telefono1,
	cc.Telefono2,
	cc.Telefono3,
	null as Telefono4,
	p.RazonSocial,
	p.ProveedorId,
	p.FechaUltimoContacto,
	est.Descripcion as Estado ,
	CASE WHEN fc.CUIT is null then 0 else 1 end as Facacop,
	p.RiesgoComercialSap, 
	isnull((select TOP 1 Situacion from rg2300 where CUIT = p.CUIT),'') as situacion 
FROM Proveedor p 
LEFT join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId 
INNER join @EmpleadoTable e on e.ComercialId = pc.ComercialId
LEFT join @ProveedorEstado PEE ON PEE.ProveedorId = p.ProveedorId
LEFT JOIN ContactoComercial CC ON CC.ProveedorId = p.ProveedorId AND CC.EsPrincipal = 1
LEFT join Estado est on est.EstadoId = pee.EstadoId
LEFT join FACACOP fc on p.CUIT = fc.CUIT

drop table #ProveedorAux
    







