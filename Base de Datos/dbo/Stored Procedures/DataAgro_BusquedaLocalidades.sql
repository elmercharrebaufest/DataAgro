CREATE PROCEDURE [dbo].[DataAgro_BusquedaLocalidades]
@filtro varchar(100) 

  
as  

BEGIN
		select l.LocalidadId as Id, l.Nombre as Localidad, l.ProvinciaId, p.Nombre as Provincia
		from Localidad l 
		inner join Provincia p on p.ProvinciaId = l.ProvinciaId
		where   
		l.Nombre like Replace(Replace(@filtro,'-',''),'.','') + '%'   
		or l.Nombre like @filtro + '%' 
		group by l.LocalidadId, l.Nombre, l.ProvinciaId, p.Nombre

END
