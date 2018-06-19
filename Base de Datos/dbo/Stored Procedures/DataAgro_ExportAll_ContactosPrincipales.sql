create PROCEDURE [dbo].[DataAgro_ExportAll_ContactosPrincipales] --26
(
	@Proveedores VARCHAR(max)
)
AS

declare @table as table(item int )

insert into @table 
select item FROM dbo.Split(@Proveedores, ',') 


	SELECT 
		p.CUIT,
		p.RazonSocial,
		CC.Nombres as Nombre,
		CC.Apellido ,
		CC.Email1 as Email,
		CC.Telefono1 as Telefono,
		convert(varchar(20),CC.FechaNacimiento,103) as FechaNacimiento,
		CC.Cargo as Profesion,
		CC.Puesto,
		
		
		(STUFF((
			SELECT ', ' + Descripcion
			FROM Interes I
			INNER JOIN ContactoComercialInteres CCI ON CCI.InteresId = I.InteresId
			WHERE CCI.ContactoComercialId = CC.ContactoComercialId
			FOR XML PATH('')
		), 1, 2, '')) AS Interes,
		
		CC.OtrosIntereses as OtrosIntereses,
		case when cc.EsPrincipal = 1 then 'X' else '' end as Principal
		
	FROM ContactoComercial CC
	INNER JOIN Proveedor p on Cc.ProveedorId =p.ProveedorId
	WHERE CC.ProveedorId in (select * from @table)
    order by cc.EsPrincipal desc, cc.ContactoComercialId asc