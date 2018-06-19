
create PROCEDURE [dbo].[DataAgro_IndicadoresComprasExportacionTorta]  
 
@MaterialId int  =null,

@CampañaId int =null,

@ComercialId int  =44

 aS

 declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargo int , IdActiveDirectory varchar(255),GrupoDeCompras int);
   

--RECURSIVIDAD
--WITH Empleados 
--( ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory,GrupoDeCompras)
--AS
--(
--	SELECT ComercialId, Apellido, Nombres, PerfilId, EmpleadorACargo, IdActiveDirectory,GrupoDeCompras
--    FROM Comercial  
--	WHERE ComercialId = @comercialId 
--	UNION ALL 
--	SELECT A.ComercialId, A.Apellido, A.Nombres, A.PerfilId, A.EmpleadorACargo, A.IdActiveDirectory,a.GrupoDeCompras
--	FROM Comercial A
--	inner join Empleados AS B on A.EmpleadorACargo = B.ComercialId
--)
--insert into @EmpleadoTable select * from Empleados

insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId

select p.cuit,cmm.toneladas as Toneladas,m.Descripcion as Material,c.Descripcion as Campaña,cast(cmm.Año as varchar(20)) as Año,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial
,case when cmm.Mes=1 then 'ENERO'
when cmm.Mes=2 then 'FEBRERO' 
when cmm.Mes=3 then 'MARZO' 
when cmm.Mes=4 then 'ABRIL' 
when cmm.Mes=5 then 'MAYO' 
when cmm.Mes=6 then 'JUNIO' 
when cmm.Mes=7 then 'JULIO' 
when cmm.Mes=8 then 'AGOSTO' 
when cmm.Mes=9 then 'SEPTIEMBRE' 
when cmm.Mes=10 then 'OCTUBRE' 
when cmm.Mes=11 then 'NOVIEMBRE' 
when cmm.Mes=12 then 'DICIEMBRE' 
else 'SIN MES'
end  as Mes,
p.RazonSocial as razonSocial,
isnull(prv.Nombre,'SIN PROVINCIA' )as Provincia

from CampañaMaterialPorMes cmm
inner join CampañaMaterial cm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join proveedor p on p.ProveedorId= cm.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join segmentacion seg on p.SegmentacionId=seg.SegmentacionId
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId
left join Localidad loc on p.LocalidadId= loc.LocalidadId
left join provincia prv on loc.ProvinciaId = prv.ProvinciaId

where   ( (@MaterialId is null) or (CM.MaterialId=@MaterialId ))

and ( (@CampañaId is null) or (cm.CampañaId = @CampañaId) )



ORDER BY P.CUIT



--[DataAgro_IndicadoresComprasExportacionTorta]   1,5,44