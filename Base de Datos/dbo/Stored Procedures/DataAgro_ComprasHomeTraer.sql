

CREATE PROCEDURE [dbo].[DataAgro_ComprasHomeTraer] --10
(
	@ComercialId INT
)
AS
BEGIN
	
	--WITH Empleados   
	--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)  
	--AS  
	--(  
	-- SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory  
	--	FROM Comercial    
	-- WHERE ComercialId = @ComercialId
	-- UNION ALL   
	--	--RECURSIVIDAD  
	-- SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory  
	-- FROM Comercial A  
	-- inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId  
	--)  

	declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
	insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId


	SELECT 
		m.Descripcion as Nombre,
		sum(cmm.Toneladas) as Toneladas,
		c.Descripcion as Campaña
	FROM CampañaMaterialPorMes cmm
	INNER JOIN CampañaMaterial cm on cm.CampañaMaterialId = cmm.CampañaMaterialId
	inner join Material m on cm.MaterialId = m.MaterialId
	inner join campaña c on c.CampañaId = m.CampañaIdActual and c.CampañaId = cm.CampañaId
	inner join @EmpleadoTable e on e.ComercialId = cmm.ComercialID
	group by m.Descripcion,c.Descripcion
	order by m.Descripcion asc


END
