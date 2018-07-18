CREATE procedure [dbo].[DataAgro_ValidarProveedor_PorComercial]

@comercialId int,
@proveedorId int= null

AS


WITH Empleados 
( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)
AS
(
	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory
    FROM Comercial  
	WHERE ComercialId = @comercialId 
	UNION ALL 
    --RECURSIVIDAD
	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory
	FROM Comercial A
	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId
)


  SELECT 1 as Val 
  into #valor
  FROM Empleados e 
  inner join ProveedorComercial pc on e.ComercialId = pc.ComercialId 
  WHERE pc.ProveedorId = @proveedorId 


if ((select COUNT(val) from #valor)> 0)
begin
   select cast(1 as int) as valor
end
else
begin
   select cast(0 as int) as valor
end