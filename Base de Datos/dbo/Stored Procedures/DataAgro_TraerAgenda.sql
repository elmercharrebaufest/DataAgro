
CREATE PROCEDURE [dbo].[DataAgro_TraerAgenda] 
 @comercialId int,
 @detalle varchar(200) = null,
 @TipoDeActividad int = null,
 @ProveedorId int,
 @fechaDesde datetime = null,
 @fechaHasta datetime = null
AS

DECLARE @TablaAux TABLE(RazonSocial VARCHAR(500),Detalle VARCHAR(MAX),TipoDeAcividad VARCHAR(200),FechaHoraActividad DATETIME,FechaHoraRecordatorio DATETIME,Apellido VARCHAR(100),NombreContacto VARCHAR(300),ActividadId INT);

--WITH Empleados 
--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory)
--AS
--(
--	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory
--    FROM Comercial  
--	WHERE ComercialId = @comercialId 
--	UNION ALL 
--    --RECURSIVIDAD
--	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory
--	FROM Comercial A
--	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId
--)

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int)
 
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId


insert into @TablaAux(RazonSocial,Detalle,TipoDeAcividad,FechaHoraActividad,FechaHoraRecordatorio,Apellido,NombreContacto,ActividadId)

select distinct 
	p.RazonSocial, 
	cast(a.Detalle as varchar(max)) as Detalle, 
	ta.Descripcion as TipoDeAcividad,
	a.FechaHoraActividad,
	ISNULL(a.FechaHoraRecordatorio,a.FechaHoraActividad) as FechaHoraRecordatorio,
	e.Apellido,
	cc.Apellido + ' ' + cc.Nombres as NombreContacto,
	a.ActividadId 
FROM @EmpleadoTable e
inner join ProveedorComercial pc on e.ComercialId = pc.ComercialId
inner join Proveedor p on p.ProveedorId = pc.ProveedorId
inner join Actividad a on a.ProveedorId = p.ProveedorId
inner join TipoActividad ta on a.TipoActividadId = ta.TipoActividadId
inner join ContactoComercial cc on cc.ProveedorId = p.ProveedorId and a.ContactoComercialId= cc.ContactoComercialId
where 1 = 1
and ((@detalle is null) or ( a.Detalle like '%' + @detalle + '%' ))
and ((@TipoDeActividad is null) or ( a.TipoActividadId = @TipoDeActividad))
and ((@ProveedorId is null) or ( a.ProveedorId = @ProveedorId))
/*and ((@fechaDesde is null) or (cast(a.FechaHoraRecordatorio as date) >= cast(@fechaDesde as date)))
and ((@fechaHasta is null) or (cast(a.FechaHoraRecordatorio as date)<= cast(@fechaHasta as date)))*/ 
--se comento pq ahora se busca por fecha de alta
and ((@fechaDesde is null) or (cast(a.FechaHoraActividad as date) >= cast(@fechaDesde as date)))
and ((@fechaHasta is null) or (cast(a.FechaHoraActividad as date)<= cast(@fechaHasta as date)))


select 
	RazonSocial,
	Detalle,
	TipoDeAcividad,
	FechaHoraActividad,
	FechaHoraRecordatorio,
	Apellido,
	NombreContacto,
	ActividadId	
FROM @TablaAux
order by case when FechaHoraRecordatorio is null then 1 else 0 end, FechaHoraRecordatorio ASC