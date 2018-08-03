
CREATE PROCEDURE [dbo].[DataAgro_ActualizarComercialHome] --42
(
	@ComercialId INT
)
AS
BEGIN
	
	declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)
	insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId



	select 
		P.CUIT,
		E.IdActiveDirectory as UsuarioDirectory
	from ProveedorComercial  PC 
	inner join @EmpleadoTable E ON E.ComercialId = PC.ComercialId
	INNER JOIN Proveedor P ON P.ProveedorId = PC.ProveedorId
	group by P.CUIT, E.IdActiveDirectory
	


END
