CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_Traer] --42
(
	@ProveedorId INT
)
AS


select distinct @ProveedorId as ProveedorId, m.MaterialId,m.descripcion as Material,camp.CampañaId,camp.descripcion as Campaña
from campo c
inner join CampoMaterial cm on c.CampoId = cm.CampoId
inner join Material m on m.materialId = cm.MaterialId 
inner join Campaña camp on  cm.CampañaId = camp.CampañaId
left join 
(
	select distinct i.proveedorId,d.MaterialId as MaterialId, i.CampañaId
	from InformeComercial i
	inner join InformeComercialProduccion d on d.InformeComercialId = i.InformeComercialId
	where i.ProveedorId = @ProveedorId
) a on cm.MaterialId = a.MaterialId and c.proveedorId = a.proveedorId

where c.ProveedorId = @ProveedorId and a.proveedorId is null and a.MaterialId is null and a.CampañaId is null