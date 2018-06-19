CREATE PROCEDURE DataAgro_BusquedaPorCUITRazonSocial
(
	@Filtro VARCHAR(150)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @FiltroRS VARCHAR(150) = @Filtro
	DECLARE @FiltroC VARCHAR(150) = REPLACE(REPLACE(@Filtro,'-',''),'.','')
	
	SELECT
		P.ProveedorId,
		P.CUIT,
		P.RazonSocial
	FROM Proveedor P
	WHERE
	(
		REPLACE(REPLACE(P.CUIT,'-',''),'.','') LIKE '%'+ @FiltroC +'%'
	OR
		P.RazonSocial LIKE '%'+ @FiltroRS +'%'
	)
	
    
    
END
