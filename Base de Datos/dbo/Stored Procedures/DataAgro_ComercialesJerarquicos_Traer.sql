
CREATE PROCEDURE [dbo].[DataAgro_ComercialesJerarquicos_Traer]   
    @comercialId VARCHAR(MAX)  
AS   


declare @table as table(item int )

insert into @table 
 select item FROM dbo.Split(@comercialId, ',') 

 SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargoId, IdActiveDirectory,GrupoDeComprasId
    FROM Comercial  
	where ComercialId in (select * from @table)