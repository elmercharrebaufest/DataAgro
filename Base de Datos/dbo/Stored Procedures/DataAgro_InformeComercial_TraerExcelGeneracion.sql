
CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_TraerExcelGeneracion] 


AS

select ic.InformeComercialId as InformeComercialId , p.CUIT as Cuit ,p.RazonSocial as RazonSocial ,c.Descripcion as Campaña,
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

from Informecomercial ic
inner join Proveedor p on ic.proveedorId =  p.ProveedorId
inner join Campaña c on ic.campañaId = c.campañaId
inner join Comercial com on ic.comercialId = com.ComercialId
where ic.EstadoId = 1
