
CREATE PROCEDURE [dbo].[DataAgro_ActualizarComercialHome] --42
(
	@ComercialId INT
)
AS
BEGIN
	
	--WITH Empleados   
	--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)  
	--AS  
	--(  
	-- SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory  
	--	FROM Comercial    
	-- WHERE ComercialId = @ComercialId
	-- UNION ALL   
	--	--RECURSIVIDAD  
	-- SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory  
	-- FROM Comercial A  
	-- inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId  
	--)  
	declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
	insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId



	select 
		P.CUIT,
		E.IdActiveDirectory as UsuarioDirectory
	from ProveedorComercial  PC 
	inner join @EmpleadoTable E ON E.ComercialId = PC.ComercialId
	INNER JOIN Proveedor P ON P.ProveedorId = PC.ProveedorId
	group by P.CUIT, E.IdActiveDirectory
	


END
