CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_TraerInformesGenerado] 
 @ProveedorId int 
AS
select ic.InformeComercialId as InformeComercialId,ic.EstadoId, est.descripcion as EstadoInforme , p.CUIT as Cuit ,p.RazonSocial as RazonSocial ,c.Descripcion as Campaña,
com.Nombres + ' ' + com.Apellido as Comercial,
(STUFF((
			SELECT ISNULL(m.Descripcion,'') +  CASE
                 WHEN ROW_NUMBER() OVER (ORDER BY (SELECT 0)) = 1 THEN '|'
                 ELSE ''
               END
			from InformeComercialProduccion ifp
			inner Join Material m on ifp.MaterialId = m.MaterialId
			where ifp.InformeComercialId = ic.InformeComercialId 
			group by m.Descripcion
			FOR XML PATH('')
		), 1, 0, '')) AS Materiales
from informecomercial ic
inner join Proveedor p on ic.proveedorId =  p.ProveedorId
inner join Campaña c on ic.campañaId = c.campañaId
inner join proveedorComercial pc on p.ProveedorId=pc.ProveedorId
inner join Comercial com on pc.comercialId = com.ComercialId
inner join InformeComercialEstado Est on est.EstadoInformeId = ic.EstadoId
where ic.proveedorId= @ProveedorId
and exists ( select 1 from InformeComercialProduccion ifp
			inner Join Material m on ifp.MaterialId = m.MaterialId 
			and ifp.InformeComercialId = ic.InformeComercialId and m.CampañaId = ic.CampañaId)