
CREATE PROCEDURE DataAgro_Zona_TraerPorComerciales

  @ComercialId Int

AS


--WITH Empleados   
--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory, GrupoDeCompras)  
--AS  
--(  
-- SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory  ,GrupoDeCompras
--    FROM Comercial    
--	WHERE ComercialId = @comercialId   
-- UNION ALL   
--    --RECURSIVIDAD  
--	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory  , a.GrupoDeCompras
--	FROM Comercial A  
--	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId  
--)  

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
 
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @comercialId

select gc.Id as Id, gc.Descripcion as Nombre
from 
( select distinct GrupoDeCompras  from @EmpleadoTable) e
inner join  GrupoDeCompras gc on e.GrupoDeCompras = gc.Id
where LEN(gc.Descripcion) > 0 
order by gc.Descripcion asc