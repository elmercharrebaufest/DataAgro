CREATE PROCEDURE [dbo].[DataAgro_Zona_TraerPorComerciales]

  @ComercialId Int

AS


WITH Empleados   
( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory, GrupoDeCompras)  
AS  
(  
 SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory  ,GrupoDeCompras
    FROM Comercial    
	WHERE ComercialId = @comercialId   
 UNION ALL   
    --RECURSIVIDAD  
	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory  , a.GrupoDeCompras
	FROM Comercial A  
	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId  
)  

select gc.Id as Id, gc.Descripcion as Nombre
from 
( select distinct GrupoDeCompras  from Empleados) e
inner join  GrupoDeCompras gc on e.GrupoDeCompras = gc.Id
where LEN(gc.Descripcion) > 0 
order by gc.Descripcion asc