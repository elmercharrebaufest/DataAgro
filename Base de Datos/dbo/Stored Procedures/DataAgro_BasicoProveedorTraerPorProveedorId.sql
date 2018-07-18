
CREATE PROCEDURE [dbo].[DataAgro_BasicoProveedorTraerPorProveedorId]
(
	 @ProveedorId INT,
	 @ComercialId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	--WITH Empleados   
	--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)  
	--AS			
	--(  
	--	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory  
	--	FROM Comercial    
	--	WHERE ComercialId = @ComercialId
	--UNION ALL   
	--	--RECURSIVIDAD  
	--	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory  
	--	FROM Comercial A  
	--	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId  
	--) 
       
	--select * 
	--into #Empleados
	--from Empleados

	
	declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
 
    insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId
	
	SELECT 
		p.ProveedorId,
		P.RazonSocial,
		P.CUIT,
		EST.Descripcion AS Estado,
		CASE WHEN FC.CUIT is null THEN 0 ELSE 1 END AS Facacop,
		P.RiesgoComercialSap,
		RG.Situacion,
		S.Descripcion Segmentacion,
		CC.Email1,
		CC.Email2,
		CC.Email3,
		NULL AS Email4,
		CC.Telefono1,
		CC.Telefono2,
		CC.Telefono3,
		NULL AS Telefono4,
		P.Observaciones,
		P.FechaUltimoContacto,
		COM.Nombres,
		COM.Apellido,
		P.Direccion,
		LOC.Nombre AS Localidad,
		PRO.Nombre AS Provincia,
		p.Provinciaid as ProvinciaId,
		CAN.Descripcion AS CanalOperacion,
		DST.Descripcion AS Destinatario,
		CND.Descripcion AS Condicion,
		ARI.Descripcion AS AreaInfluencia,
		P.Intermediario,
		P.Calificacion,
		isnull(gc.Descripcion,'') AS GrupoDeCompras,
		P.CodigoPostal,
		cc.TipoTelefono1Id,
		cc.TipoTelefono2Id,
		cc.TipoTelefono3Id,
		null as TipoTelefono4Id,
		CASE WHEN (LEN(ISNULL(cc.nombres,'')) = 0 AND LEN(ISNULL(cc.apellido,'')) = 0) THEN 'No Posee' ELSE ISNULL(cc.Nombres,'') + ' ' + ISNULL(CC.Apellido,'') END as NombreReferente
	into #ProveedorAux
	FROM Proveedor P
	LEFT JOIN ProveedorCanalOperacion PCO ON PCO.ProveedorId = P.ProveedorId
	LEFT JOIN ProveedorComercial PC ON PC.ProveedorId = P.ProveedorId
	LEFT JOIN ProveedorCondicion PCN ON PCN.ProveedorId = P.ProveedorId
	LEFT JOIN ProveedorDestinatario PD ON PD.ProveedorId = P.ProveedorId
	LEFT JOIN Estado EST ON EST.EstadoId = P.EstadoId
	LEFT JOIN FACACOP FC ON FC.CUIT = P.CUIT
	LEFT JOIN RG2300 RG ON RG.CUIT = P.CUIT
	LEFT JOIN Segmentacion S ON S.SegmentacionId = P.SegmentacionId
	LEFT JOIN Comercial COM ON COM.ComercialId = PC.ComercialId
	LEFT JOIN GrupoDeCompras gc on com.GrupoDeCompras = gc.Id
	LEFT JOIN Localidad LOC ON LOC.LocalidadId = P.LocalidadId
	LEFT JOIN Provincia PRO ON PRO.ProvinciaId = LOC.ProvinciaId
	LEFT JOIN CanalOperacion CAN ON CAN.CanalOperacionId = PCO.CanalOperacionId
	LEFT JOIN Destinatario DST ON DST.DestinatarioId = PD.DestinatarioId
	LEFT JOIN Condicion CND ON CND.CondicionId = PCN.CondicionId
	LEFT JOIN AreaInfluencia ARI ON ARI.AreaInfluenciaId = P.AreaInfluenciaId
	LEFT JOIN ContactoComercial CC ON CC.ProveedorId = P.ProveedorId AND CC.EsPrincipal = 1
	WHERE P.ProveedorId = @ProveedorId
    

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
										(select EstadoId From Proveedor PP WHERE PP.ProveedorId = t.ProveedorId)
								ENd
							ENd
					ENd
			ENd
		ENd as Estado
		from #ProveedorAux t 
	left join ProveedorEstado pe on t.ProveedorId = pe.ProveedorId and pe.ComercialId in (select ComercialId from  @EmpleadoTable)
	--group by pe.ProveedorId




	SELECT 
		P.RazonSocial,
		P.CUIT,
		EST.Descripcion AS Estado,
		CASE WHEN FC.CUIT is null THEN 0 ELSE 1 END AS Facacop,
		P.RiesgoComercialSap,
		RG.Situacion,
		S.Descripcion Segmentacion,
		CC.Email1,
		CC.Email2,
		CC.Email3,
		NULL AS Email4,
		CC.Telefono1,
		CC.Telefono2,
		CC.Telefono3,
		NULL AS Telefono4,
		P.Observaciones,
		P.FechaUltimoContacto,
		COM.Nombres,
		COM.Apellido,
		P.Direccion,
		LOC.Nombre AS Localidad,
		PRO.Nombre AS Provincia,
		p.Provinciaid as ProvinciaId,
		CAN.Descripcion AS CanalOperacion,
		DST.Descripcion AS Destinatario,
		CND.Descripcion AS Condicion,
		ARI.Descripcion AS AreaInfluencia,
		P.Intermediario,
		P.Calificacion,
		isnull(gc.Descripcion,'') AS GrupoDeCompras,
		P.CodigoPostal,
		cc.TipoTelefono1Id,
		cc.TipoTelefono2Id,
		cc.TipoTelefono3Id,
		null as TipoTelefono4Id,
		CASE WHEN (LEN(ISNULL(cc.nombres,'')) = 0 AND LEN(ISNULL(cc.apellido,'')) = 0) THEN 'No Posee' ELSE ISNULL(cc.Nombres,'') + ' ' + ISNULL(CC.Apellido,'') END as NombreReferente,
		p.clienteMOA as ClienteMOA
	FROM Proveedor P
	LEFT JOIN ProveedorCanalOperacion PCO ON PCO.ProveedorId = P.ProveedorId
	LEFT JOIN ProveedorComercial PC ON PC.ProveedorId = P.ProveedorId
	LEFT JOIN ProveedorCondicion PCN ON PCN.ProveedorId = P.ProveedorId
	LEFT JOIN ProveedorDestinatario PD ON PD.ProveedorId = P.ProveedorId
	LEFT JOIN @ProveedorEstado PEE On PEE.ProveedorId = P.ProveedorId
	LEFT JOIN Estado EST ON EST.EstadoId = PEE.EstadoId
	LEFT JOIN FACACOP FC ON FC.CUIT = P.CUIT
	LEFT JOIN RG2300 RG ON RG.CUIT = P.CUIT
	LEFT JOIN Segmentacion S ON S.SegmentacionId = P.SegmentacionId
	LEFT JOIN Comercial COM ON COM.ComercialId = PC.ComercialId
	LEFT JOIN GrupoDeCompras gc on com.GrupoDeCompras = gc.Id
	LEFT JOIN Localidad LOC ON LOC.LocalidadId = P.LocalidadId
	LEFT JOIN Provincia PRO ON PRO.ProvinciaId = LOC.ProvinciaId
	LEFT JOIN CanalOperacion CAN ON CAN.CanalOperacionId = PCO.CanalOperacionId
	LEFT JOIN Destinatario DST ON DST.DestinatarioId = PD.DestinatarioId
	LEFT JOIN Condicion CND ON CND.CondicionId = PCN.CondicionId
	LEFT JOIN AreaInfluencia ARI ON ARI.AreaInfluenciaId = P.AreaInfluenciaId
	LEFT JOIN ContactoComercial CC ON CC.ProveedorId = P.ProveedorId AND CC.EsPrincipal = 1
	WHERE P.ProveedorId = @ProveedorId

	drop table #ProveedorAux
	--drop table #Empleados   
END

