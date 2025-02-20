CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_Traer] --42
(
	@ProveedorId INT
)
AS

select ProveedorId,MaterialId,Material,CampañaId, Campaña   
from (
select distinct c.ProveedorId as ProveedorId, m.MaterialId,m.Descripcion as Material,camp.CampañaId,camp.Descripcion as Campaña
from Campo c  
inner join CampoMaterial cm on c.CampoId = cm.CampoId  
inner join Material m on m.MaterialId = cm.MaterialId   
inner join Campaña camp on  cm.CampañaId = camp.CampañaId  
 
where c.ProveedorId = @ProveedorId  

union all
 select i.ProveedorId,d.MaterialId as MaterialId,m.Descripcion as Material,camp.CampañaId, camp.Descripcion as Campaña  
 from InformeComercial i  
 inner join InformeComercialProduccion d on d.InformeComercialId = i.InformeComercialId  
 inner join Material m on m.MaterialId = d.MaterialId 
 inner join Campaña camp on  i.CampañaId = camp.CampañaId  
 where i.ProveedorId = @ProveedorId  

 ) a
 group by  ProveedorId,MaterialId,Material,CampañaId, Campaña  
 having count(*)=1