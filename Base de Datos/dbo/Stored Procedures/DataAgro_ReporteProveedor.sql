
CREATE PROCEDURE [dbo].[DataAgro_ReporteProveedor] --'200','27'
(
	@Valor VARCHAR(150),
	@ComercialId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	SELECT 
		P.ProveedorId,
		P.CUIT,
		P.RazonSocial,
		P.EstadoId,
		Es.Descripcion Estado,
		P.Alias,
		(STUFF((
			SELECT ', ' + ISNULL(CC.Nombres,'') + ' ' + ISNULL(CC.Apellido,'')
			FROM Comercial CC
			WHERE CC.ComercialId = C.ComercialId
			FOR XML PATH('')
		), 1, 2, '')) AS Comerciales
	into #ProveedorAux
	FROM Proveedor p  
	LEFT join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId 
	LEFT JOIN Comercial C ON C.ComercialId = PC.ComercialId
	LEFT JOIN Estado Es ON Es.EstadoId = P.EstadoId
    WHERE @Valor is null 
	OR
	P.CUIT LIKE '%'+@Valor+'%'
	OR
	P.Alias LIKE '%'+@Valor+'%'
	OR
	P.RazonSocial LIKE '%'+@Valor+'%'
	OR
	C.Nombres LIKE '%'+ @Valor+'%'
	OR
	C.Apellido LIKE '%' +@Valor+'%'



	DECLARE @ProveedorEstado TABLE(ProveedorId INT, EstadoId INT)

	INSERT INTO @ProveedorEstado 
	select 
		distinct t.ProveedorId , 
		case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 4) 
		then 4 else 
			case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 5) 
			then 5  else
					case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 1) 
					then 1  else
							case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 2) 
							then 2 else
								case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.estadoId = 3) 
								then 3  else
										(select EstadoId From Proveedor PP WHERE PP.ProveedorId = t.ProveedorId)
								ENd
							ENd
					ENd
			ENd
		ENd as Estado
		from #ProveedorAux t 
	LEFT join ProveedorEstado pe on t.ProveedorId = pe.ProveedorId
	--where pe.ComercialId in (select ComercialId from  #Empleados)
	--group by pe.ProveedorId





    select 
		P.ProveedorId,
		P.CUIT,
		P.RazonSocial,
		P.EstadoId,
		E.Descripcion Estado,
		P.Alias,
		(STUFF((
			SELECT ', ' + ISNULL(CC.Nombres,'') + ' ' + ISNULL(CC.Apellido,'')
			FROM Comercial CC
			WHERE CC.ComercialId = C.ComercialId
			FOR XML PATH('')
		), 1, 2, '')) AS Comerciales
	from Proveedor P
	left join ProveedorComercial PC ON PC.ProveedorId = P.ProveedorId
	LEFT JOIN Comercial C ON C.ComercialId = PC.ComercialId
	INNER JOIN @ProveedorEstado PE ON PE.ProveedorId = p.ProveedorId
	LEFT JOIN Estado E ON E.EstadoId = PE.EstadoId

	WHERE @Valor is null 
	OR
	P.CUIT LIKE '%'+@Valor+'%'
	OR
	P.Alias LIKE '%'+@Valor+'%'
	OR
	P.RazonSocial LIKE '%'+@Valor+'%'
	OR
	C.Nombres LIKE '%'+ @Valor+'%'
	OR
	C.Apellido LIKE '%' +@Valor+'%'
	ORDER BY p.ProveedorId

	
	drop table #ProveedorAux




END
