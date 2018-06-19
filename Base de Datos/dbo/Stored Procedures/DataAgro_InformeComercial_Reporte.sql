CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_Reporte] --42
(
	@Cuit Varchar(20),
	@ComercialID Int,
	@MaterialID int
)
AS


select * from
(
select 
	p.cuit,
	p.razonSocial,
	null FechaDeGeneracion,
	ccc.Nombres + ' ' + ccc.Apellido Comercial,
	'Pendiente de Generación' Estado,
	m.Descripcion Material,
	'' Observaciones
from campo c
inner join CampoMaterial cm on c.CampoId = cm.CampoId
inner join Material m on m.materialId = cm.MaterialId and m.CampañaIdActual = cm.CampañaId
inner join Campaña camp on  m.CampañaIdActual = camp.CampañaId
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
AND (@ComercialID is null or CCC.ComercialId = @ComercialID)

UNION

select 
	p.Cuit,
	p.RazonSocial,
	ic.FechaAlta FechaDeGeneracion,
	c.Nombres + ' ' + c.Apellido Comercial,
	ie.Descripcion Estado,
	m.Descripcion Material,
	icp.MensajeSap Observaciones
from InformeComercial ic
inner join informecomercialestado ie on ie.EstadoInformeId = ic.EstadoId
inner join proveedor p on p.proveedorid = ic.proveedorid
inner join comercial c on c.comercialid = ic.comercialid
inner JOIN InformeComercialProduccion icp on icp.InformeComercialId = IC.InformeComercialId
inner join Material m on m.MaterialId = icp.MaterialId
WHERE (@cuit is null or P.CUIT like '%'+@Cuit+'%')
AND (@ComercialID is null or c.ComercialId = @ComercialID)
and (@MaterialId is null or @MaterialId = icp.MaterialId)
) A
Order by A.RazonSocial