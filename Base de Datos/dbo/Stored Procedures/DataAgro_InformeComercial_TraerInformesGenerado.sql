CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_TraerInformesGenerado] 
 @ProveedorId int 
AS
select ic.InformeComercialId as InformeComercialId,ic.EstadoId, est.Descripcion as EstadoInforme , p.CUIT as Cuit ,p.RazonSocial as RazonSocial ,c.Descripcion as Campaña,
com.Nombres + ' ' + com.Apellido as Comercial, ic.FechaAlta,
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
from InformeComercial ic
inner join Proveedor p on ic.ProveedorId =  p.ProveedorId
inner join Campaña c on ic.CampañaId = c.CampañaId
inner join ProveedorComercial pc on p.ProveedorId=pc.ProveedorId
inner join Comercial com on pc.ComercialId = com.ComercialId
inner join InformeComercialEstado Est on est.EstadoInformeId = ic.EstadoId
where ic.ProveedorId= @ProveedorId
and exists ( select 1 from InformeComercialProduccion ifp
			inner Join Material m on ifp.MaterialId = m.MaterialId 
			and ifp.InformeComercialId = ic.InformeComercialId)