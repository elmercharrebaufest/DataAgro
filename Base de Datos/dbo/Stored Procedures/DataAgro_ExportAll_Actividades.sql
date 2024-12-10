create procedure [dbo].[DataAgro_ExportAll_Actividades] --'993' 
 @Proveedores VARCHAR(max)
as


declare @table as table(item int )

 insert into @table 
 select Item FROM dbo.Split(@Proveedores, ',') 


 SELECT  
		pr.CUIT as Cuit,
		pr.RazonSocial as RazonSocial,
		TA.Descripcion AS TipoActividad,    
		A.Detalle as DetalleContacto,  
		A.asunto as Asunto,
		cast(A.FechaHoraRecordatorio as varchar(50)) as FechaDesde,
		cast(datepart(hour,A.FechaHoraRecordatorio) as varchar(50)) as HoraDesde, 
		cast(A.FechaHoraRecordatorioFin as varchar(50)) as FechaHasta,  
		cast(datepart(hour,A.FechaHoraRecordatorioFin) as varchar(50)) as HoraHasta/*, 
		
		TA.Descripcion AS TipoActividad,  

		CC.ComercialId,  

		A.ContactoComercialId,  

		isnull(CON.Nombres + ' ' + CON.Apellido,'') As ContactoComercial  
		*/
 FROM Actividad A  
	 LEFT JOIN TipoActividad TA ON TA.TipoActividadId = A.TipoActividadId  
	 LEFT JOIN Proveedor PR ON A.ProveedorId = PR.ProveedorId
	 LEFT JOIN Comercial CC ON CC.ComercialId = A.ComercialId  
	 LEFT JOIN ContactoComercial CON ON CON.ContactoComercialId = A.ContactoComercialId  
	 WHERE A.ProveedorId  in (select * from @table)