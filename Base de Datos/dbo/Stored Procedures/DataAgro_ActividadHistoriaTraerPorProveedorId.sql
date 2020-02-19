--Hecho en repositorio: ConsultaActividadHistoriaTraerPorProveedorId
CREATE PROCEDURE [dbo].[DataAgro_ActividadHistoriaTraerPorProveedorId]  

(  

  @ProveedorId INT,

  @detalle varchar(200) = null,

  @Actividades varchar(max) = null

)  

AS  

BEGIN  

 SET NOCOUNT ON;  

  declare @ActividadesSecuencia TABLE (Item INT)     

  insert into @ActividadesSecuencia (Item) select Item  from dbo.Split (@Actividades,'|') 


 SELECT  

  A.ActividadId,  

  A.Detalle,  

  A.FechaHoraActividad,  

  A.FechaHoraRecordatorio, 
  A.FechaHoraRecordatorioFin,  
  A.asunto,

  TA.Descripcion AS TipoActividad,  

  CC.ComercialId,  

  A.ContactoComercialId,  

  isnull(CON.Nombres + ' ' + CON.Apellido,'') As ContactoComercial  

 FROM Actividad A  

 LEFT JOIN TipoActividad TA ON TA.TipoActividadId = A.TipoActividadId  

 LEFT JOIN Comercial CC ON CC.ComercialId = A.ComercialId  

 LEFT JOIN ContactoComercial CON ON CON.ContactoComercialId = A.ContactoComercialId  

 WHERE A.ProveedorId = @ProveedorId  

 and (( @Actividades is null)  or (@Actividades= '0' and a.ActividadId is not null) 
	or (exists ( select 1 from @ActividadesSecuencia where Item = a.TipoActividadId)))

 and ((@detalle is null) or ( a.Detalle like '%' + @detalle + '%' ))

 Order by A.FechaHoraActividad DESC  

      

END 