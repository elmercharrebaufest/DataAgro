CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_TraerAlmacenamiento] 
 @ProveedorId int,
 @CampañaId int,
 @Id int
AS
select  @Id as InformeComercialId,a.LocalidadId as LocalidadId
		,cm.HasArrendadas as Propio,Sum(cast(cm.Toneladas as decimal(18,2))) as Toneladas
from Acopio a 
inner join AcopioCampaña cm on a.AcopioId = cm.AcopioId
where a.ProveedorId=@ProveedorId and cm.CampañaId =@CampañaId
group by a.LocalidadId,cm.HasArrendadas
