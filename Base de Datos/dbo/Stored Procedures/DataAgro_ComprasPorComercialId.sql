
CREATE PROCEDURE [dbo].[DataAgro_ComprasPorComercialId]
(
	@comercialId int,
	@CampañaMaterialId int
)
AS
BEGIN
	SET NOCOUNT ON;


	
	--declare @comercialId_ INT = @comercialId;

	--WITH Empleados 
	--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)
	--AS
	--(
	--SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory
	--FROM Comercial  
	--WHERE ComercialId = @comercialId_ 
	--UNION ALL 
	----RECURSIVIDAD
	--SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory
	--FROM Comercial A
	--inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId
	--)

	declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
	insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId


	select
	CampañaMaterialPorMesId,
	NroItem,
	Mes,
	Año,
	Toneladas,
	CampañaMaterialId,
	ComercialId
	from CampañaMaterialPorMes
	where CampañaMaterialId = @CampañaMaterialId
	and Toneladas > 0
	and ComercialId in (Select ComercialId From @EmpleadoTable)





END
