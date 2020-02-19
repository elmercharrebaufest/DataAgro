CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_TraerMaterialesAModificar] 
(
	@InformeComercialId INT
)
AS
declare @ProveedorId int = null, @CampañaId int = null;

select @ProveedorId= ProveedorId , @CampañaId = CampañaId from [dbo].InformeComercial  where InformeComercialId =@InformeComercialId

select distinct  m.MaterialId,m.descripcion as Material,
(case when exists(
	select 1
	from InformeComercial i
	inner join InformeComercialProduccion d on d.InformeComercialId = i.InformeComercialId
	where i.ProveedorId = @ProveedorId and m.materialId = d.materialId and i.CampañaId =m.CampañaId
) then cast(1 as bit) else cast(0 as bit) end) as Seleccionado
from campo c
inner join CampoMaterial cm on c.CampoId = cm.CampoId
inner join Material m on m.materialId = cm.MaterialId and m.CampañaId = cm.CampañaId and  m.CampañaId = @CampañaId
inner join Campaña camp on  m.CampañaId = camp.CampañaId
left join 
(
	select distinct i.informeComercialId , i.proveedorId,d.MaterialId as MaterialId, i.CampañaId
	from InformeComercial i
	inner join InformeComercialProduccion d on d.InformeComercialId = i.InformeComercialId
	where i.ProveedorId = @ProveedorId
) a on cm.MaterialId = a.MaterialId and cm.CampañaId = a.CampañaId and c.proveedorId = a.proveedorId
where c.ProveedorId = @ProveedorId and ((a.proveedorId is null and a.MaterialId is null and a.CampañaId is null) or(informeComercialId = @InformeComercialId ))
