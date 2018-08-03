CREATE PROCEDURE [dbo].[DataAgro_TraerProveedorPorComercial]

 @comercialId int
 
AS



WITH Empleados 
( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargoId, IdActiveDirectory)
AS
(
	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargoId, IdActiveDirectory
    FROM Comercial  
	WHERE ComercialId = @comercialId 
	UNION ALL 
    --RECURSIVIDAD
	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargoId, A.IdActiveDirectory
	FROM Comercial A
	inner join Empleados AS B on A.EmpleadorACargoId = B.ComercialId
)

select p.RazonSocial,p.ProveedorId
FROM Empleados e
inner join ProveedorComercial pc on e.ComercialId = pc.ComercialId
inner join Proveedor p on p.ProveedorId = pc.ProveedorId