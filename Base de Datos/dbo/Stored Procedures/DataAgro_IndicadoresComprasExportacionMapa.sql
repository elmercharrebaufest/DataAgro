
CREATE PROCEDURE [dbo].[DataAgro_IndicadoresComprasExportacionMapa]  

@ProvinciaId int =null  ,

@SegmentacionId VARCHAR(max) ,

@MaterialId int =null  ,

@CampañaId int =null ,
@ComercialId int =null,
@ComercialGenerador Int=null  

as


 
declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);

declare @SegmentacionSecuencia TABLE (Item INT)    
declare @ProvinciaSecuencia TABLE (Item INT) 

if (@ProvinciaId is not null)
begin 
	if (@ProvinciaId = 0 or @ProvinciaId = 1)
	begin 
	   insert into @ProvinciaSecuencia (item) values(0)
	   insert into @ProvinciaSecuencia (item) values(1)
	end
	else
		insert into @ProvinciaSecuencia (item) values(@ProvinciaId)

end

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;


insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador
 
select p.cuit,cmm.toneladas as Toneladas,m.Descripcion as Material,c.Descripcion as Campaña,cast(cmm.Año as varchar(20)) as Año,prv.Nombre as Provincia,emp.Apellido + ' ' + emp.Nombres as Comercial
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
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación,
p.RazonSocial as razonSocial

from CampañaMaterialPorMes cmm
inner join CampañaMaterial cm on cm.CampañaMaterialId=cmm.CampañaMaterialId
inner join proveedor p on p.ProveedorId= cm.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join Localidad loc on p.LocalidadId= loc.LocalidadId
inner join provincia prv on loc.ProvinciaId = prv.ProvinciaId
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId
inner join segmentacion seg on p.segmentacionId=seg.segmentacionId
where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))
and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))
and ((@comercialId is null) or (pc.ComercialId= @comercialId))
and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))
and (( @ProvinciaId is null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.provinciaId)))
and p.LocalidadId is not null

ORDER BY P.CUIT