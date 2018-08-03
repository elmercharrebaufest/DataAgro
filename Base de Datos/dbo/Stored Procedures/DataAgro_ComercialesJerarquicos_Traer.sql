
CREATE PROCEDURE [dbo].[DataAgro_ComercialesJerarquicos_Traer]   
    @comercialId int  
AS   

declare @perfilIdDirector int = 3;
declare @perfilIdVisualizador int = 4;
declare @perfilIdAdministrativo int = 5;

declare @perfilId int;
set @perfilId = (select PerfilId from Comercial where ComercialId = @comercialId);


--1 Paso Averiguar el perfil de comercial
--2 Si el comercial es director, jefe o comercial sigue igual
--3 Si es administrativo o visualizador, es buscar el perfil de director y realizar las busqueda con ese perfil

declare @comercialABuscar int;

if (@perfilId = @perfilIdVisualizador OR @perfilId = @perfilIdAdministrativo)
begin
	set @comercialABuscar =(select comercialId from comercial where PerfilId = @perfilIdDirector)
end 
else
begin 
	set @comercialABuscar  = @comercialId
end

--RECURSIVIDAD
;WITH Empleados 
( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargoId, IdActiveDirectory,GrupoDeComprasId)
AS
(
	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargoId, IdActiveDirectory,GrupoDeComprasId
    FROM Comercial  
	WHERE ComercialId = @comercialABuscar 
	UNION ALL 
	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargoId, A.IdActiveDirectory,a.GrupoDeComprasId
	FROM Comercial A
	inner join Empleados AS B on A.EmpleadorACargoId = B.ComercialId
)


select * from Empleados