
CREATE PROCEDURE [dbo].[DataAgro_ComprasPorComercialId]
(
	@ComercialId varchar(max),
	@CampañaMaterialId int
)
AS
BEGIN
	SET NOCOUNT ON;

	declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)
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
