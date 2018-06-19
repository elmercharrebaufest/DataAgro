CREATE PROCEDURE [dbo].[DataAgro_ActividadTraerPorProveedorId]
(
	 @ProveedorId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.ActividadId,
		A.Detalle,
		A.FechaHoraActividad,
		A.FechaHoraRecordatorio,
		TA.Descripcion AS TipoActividad,
		CC.ComercialId,
		A.ContactoComercialId,
		isnull(CON.Nombres + ' ' + CON.Apellido,'') As ContactoComercial
	FROM Actividad A
	LEFT JOIN TipoActividad TA ON TA.TipoActividadId = A.TipoActividadId
	LEFT JOIN Comercial CC ON CC.ComercialId = A.ComercialId
	LEFT JOIN ContactoComercial CON ON CON.ContactoComercialId = A.ContactoComercialId
	WHERE A.ProveedorId = @ProveedorId
	AND A.FechaHoraRecordatorio >= GETDATE()
	Order by A.FechaHoraRecordatorio
    
END
