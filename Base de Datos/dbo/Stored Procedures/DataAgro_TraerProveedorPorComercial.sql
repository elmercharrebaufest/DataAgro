
CREATE PROCEDURE [dbo].[DataAgro_TraerProveedorPorComercial]

 @comercialId int
 
AS

--WITH Empleados 
--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)
--AS
--(
--	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory
--    FROM Comercial  
--	WHERE ComercialId = @comercialId 
--	UNION ALL 
--    --RECURSIVIDAD
--	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory
--	FROM Comercial A
--	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId
--)

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
 
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @comercialId

select p.RazonSocial,p.ProveedorId
FROM @EmpleadoTable e
inner join ProveedorComercial pc on e.ComercialId = pc.ComercialId
inner join Proveedor p on p.ProveedorId = pc.ProveedorId