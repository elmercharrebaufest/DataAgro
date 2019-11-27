

create procedure [dbo].[DataAgro_ExportAll_Contacto] --'1,2,3',44
	@Proveedores VARCHAR(max),
	 @ComercialId VARCHAR(max)
as

--set @Proveedores ='1,2,3'
--set @ComercialId = 44

declare @table as table(item int );


declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)
 
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId 


create table #ProveedorAux (ProveedorId int,CUIT varchar(20),RazonSocial varchar(100),
RiesgoComercialSap   varchar(255), Estado  varchar(255), Situacion  varchar(255),Faccop int, Calificacion int,
Segmentacion  varchar(255),Domicilio  varchar(255),Localidad varchar(255),Provincia varchar(255),CodPostal varchar(255),
CanalDeOperacion varchar(255),Destinatario varchar(255),Condicion varchar(255),Intermediario varchar(255),
AreaDeInfluencia varchar(10),Comentario varchar(MAX),Comercial varchar(255),ClienteMoa varchar(255), 
Zona varchar(255), FechaAlta DATETIME null, Clasificacion varchar(255), TipoBoleto varchar(255), Bolsa varchar(255),
Consignatario varchar(255))


 insert into @table 
 select item FROM dbo.Split(@Proveedores, ',') 


 insert into #ProveedorAux(ProveedorId,CUIT,RazonSocial,Estado, Calificacion,
 Segmentacion,Domicilio,Localidad,Provincia,CodPostal,CanalDeOperacion,Destinatario,Condicion,Intermediario, AreaDeInfluencia,Comentario,Comercial
 ,ClienteMoa,Zona,FechaAlta, Clasificacion, TipoBoleto, Bolsa, Consignatario)

 select  
	p.ProveedorId,
	p.CUIT as CUIT, 
	p.RazonSocial as RazonSocial, 
	NULL AS Estado, 
	p.Calificacion,
	s.Descripcion as Segmentacion,
	p.Direccion as Domicilio,
	l1.Nombre ,
	p1.Nombre, 
	p.CodigoPostal,
	(STUFF((
		SELECT ', ' + Descripcion
		FROM CanalOperacion COO
		inner join ProveedorCanalOperacion  pcoo on coo.CanalOperacionId = pcoo.CanalOperacionId
		WHERE pcoo.ProveedorId = P.ProveedorId
		FOR XML PATH('')
	), 1, 2, '')),
	(STUFF((
		SELECT ', ' + Descripcion
		FROM Destinatario DEE
		inner join ProveedorDestinatario pdd on pdd.DestinatarioId = DEE.DestinatarioId
		WHERE pdd.ProveedorId = P.ProveedorId
		FOR XML PATH('')
	), 1, 2, '')),
	(STUFF((
		SELECT ', ' + Descripcion
		FROM Condicion coo
		inner join ProveedorCondicion cdd on cdd.CondicionId = coo.CondicionId
		WHERE cdd.ProveedorId = P.ProveedorId
		FOR XML PATH('')
	), 1, 2, '')),
	p.Intermediario,
	ARI.Descripcion AS AreaInfluencia,
	p.Observaciones,
	c.Apellido + ' ' + c.Nombres as ComercialACargo, 
	case when p.ClienteMOA = 1 then 'SI' else 'NO' end,
	gc.Descripcion,
	p.FechaAlta,
	claCP.Descripcion,
	boletocn.Descripcion,
	bolsacn.Descripcion,
	case when p.Consignatario = 1 then 'SI' else 'NO' end

  from Proveedor p
 inner join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId
 inner join Comercial c on pc.ComercialId = c.ComercialId
 inner join GrupoDeCompras gc on c.GrupoDeComprasId = gc.Id
 inner join Segmentacion s on p.SegmentacionId = s.SegmentacionId
 left join Localidad l1 on p.LocalidadId = l1.LocalidadId
 left join Provincia p1 on l1.ProvinciaId = p1.ProvinciaId
 /*left join ProveedorCanalOperacion  pco on p.ProveedorId = pco.ProveedorId
 left join CanalOperacion co on pco.CanalOperacionId = co.CanalOperacionId
 left join ProveedorDestinatario  pd on p.ProveedorId = pd.ProveedorId
 left join Destinatario de on pd.DestinatarioId = de.DestinatarioId
 left join ProveedorCondicion  pcn on p.ProveedorId = pcn.ProveedorId
 left join Condicion cd on pcn.CondicionId = cd.CondicionId*/
 left join FACACOP f on p.CUIT = f.CUIT
 LEFT JOIN ContactoComercial CC ON CC.ProveedorId = p.ProveedorId and cc.EsPrincipal = 1
 LEFT JOIN AreaInfluencia ARI ON ARI.AreaInfluenciaId = P.AreaInfluenciaId
 left join ClasificacionCompraNet claCP on claCP.Id = P.ClasificacionCompraNetId
 left join BoletoCompraNet boletocn on boletocn.Id = p.BoletoCompraNetId
 left join BolsaCompraNet bolsacn on bolsacn.Id = p.BolsaCompraNetId

 where p.ProveedorId in (select * from @table)
 order by p.RazonSocial desc

 update #ProveedorAux
 set Estado = (select Descripcion from Estado where EstadoId =est.Estado )
 from (
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
from #ProveedorAux t ) est
where  est.ProveedorId = #ProveedorAux.ProveedorId

select  
	CUIT,
	RazonSocial,
	Estado,
	Calificacion,
	Segmentacion,
	Domicilio,
	Localidad,
	Provincia,
	CodPostal,
	CanalDeOperacion,
	Destinatario,
	Condicion,
	Intermediario,
	AreaDeInfluencia,
	Comentario,
	Comercial,
	ClienteMoa,
	Zona,
	FechaAlta,
	Clasificacion,
	TipoBoleto,
	Bolsa,
	Consignatario
from #ProveedorAux PA

drop table #ProveedorAux

--drop table @EmpleadoTable