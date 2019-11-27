CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_Reporte] 
(
	@Cuit Varchar(20) =null,
	@ComercialID Int=null,
	@ComercialGenerador VARCHAR(MAX)=null,
	@MaterialID int=null,
	@EstadoId int=null
)
AS

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador;

select * from
(
select 
	p.cuit,
	p.razonSocial,
	null FechaDeGeneracion,
	ccc.Nombres + ' ' + ccc.Apellido Comercial,
	'Pendiente de Generación' Estado,
	m.Descripcion Material,
	'' Observaciones,
	0 InformeComercialId
from campo c
inner join CampoMaterial cm on c.CampoId = cm.CampoId
inner join Material m on m.materialId = cm.MaterialId and m.CampañaId = cm.CampañaId
inner join Campaña camp on  m.CampañaId = camp.CampañaId
left join 
(
	select distinct i.proveedorId,d.MaterialId as MaterialId, i.CampañaId
	from InformeComercial i
	inner join InformeComercialProduccion d on d.InformeComercialId = i.InformeComercialId
) a on cm.MaterialId = a.MaterialId and cm.CampañaId = a.CampañaId and c.proveedorId = a.proveedorId
inner join proveedor p on p.Proveedorid = c.Proveedorid
inner join proveedorcomercial pc on pc.proveedorid = p.proveedorid
inner join Comercial ccc on ccc.ComercialId = pc.ComercialId
where a.proveedorId is null 
and a.MaterialId is null 
and a.CampañaId is null
AND (@MaterialID is null or cm.MaterialId = @MaterialId)
AND (@cuit is null or P.CUIT like '%'+@Cuit+'%')
AND (@ComercialGenerador is null or exists(select 1 from @EmpleadoTable e where e.ComercialId =  ccc.comercialId) )
AND (@ComercialID is null or @ComercialID =  ccc.ComercialId )
AND (@EstadoID is null or @EstadoID = -1)

UNION

select 
	p.Cuit,
	p.RazonSocial,
	ic.FechaAlta FechaDeGeneracion,
	c.Nombres + ' ' + c.Apellido Comercial,
	ie.Descripcion Estado,
	m.Descripcion Material,
	icp.MensajeSap Observaciones,
	ic.InformeComercialId InformeComercialId
from InformeComercial ic
inner join informecomercialestado ie on ie.EstadoInformeId = ic.EstadoId
inner join proveedor p on p.proveedorid = ic.proveedorid
inner join proveedorcomercial pc on pc.proveedorid = p.proveedorid
inner join comercial c on c.comercialid = pc.comercialid
inner JOIN InformeComercialProduccion icp on icp.InformeComercialId = IC.InformeComercialId
inner join Material m on m.MaterialId = icp.MaterialId
WHERE (@cuit is null or P.CUIT like '%'+@Cuit+'%')
AND (@ComercialGenerador is null or exists(select 1 from @EmpleadoTable e where e.ComercialId =  c.ComercialId) )
AND (@ComercialID is null or c.comercialid = @ComercialID )
and (@MaterialId is null or @MaterialId = icp.MaterialId)
AND (@EstadoID is null or @EstadoID = ic.EstadoId)
) A
Order by A.RazonSocial