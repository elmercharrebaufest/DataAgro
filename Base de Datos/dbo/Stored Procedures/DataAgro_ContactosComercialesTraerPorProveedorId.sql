CREATE PROCEDURE [dbo].[DataAgro_ContactosComercialesTraerPorProveedorId]
(
	 @ProveedorId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		CC.Nombres,
		CC.Apellido,
		CC.Puesto,
		CC.Cargo,
		CC.Telefono1,
		CC.Telefono2,
		CC.Telefono3,
		CC.Email1,
		CC.Email2,
		CC.Email3,
		CC.FechaNacimiento,
		(STUFF((
			SELECT ', ' + Descripcion
			FROM Interes I
			INNER JOIN ContactoComercialInteres CCI ON CCI.InteresId = I.InteresId
			WHERE CCI.ContactoComercialId = CC.ContactoComercialId
			FOR XML PATH('')
		), 1, 2, '')) AS Interes,
		(STUFF((
			SELECT ', ' + cast(I.InteresId as varchar(100))
			FROM Interes I
			INNER JOIN ContactoComercialInteres CCI ON CCI.InteresId = I.InteresId
			WHERE CCI.ContactoComercialId = CC.ContactoComercialId
			FOR XML PATH('')
		), 1, 2, '')) AS InteresId,
		CC.OtrosIntereses,
		CC.TipoTelefono1Id,
		cc.TipoTelefono2Id,
		cc.TipoTelefono3Id,
		cc.ContactoComercialId,
		cc.EsPrincipal,
		CC.CompraNet,
		CC.Cupo,
		cc.Boleto,
		cc.ContactoComercialId
	FROM ContactoComercial CC
	WHERE CC.ProveedorId = @ProveedorId
    order by cc.EsPrincipal desc, cc.ContactoComercialId asc
END
