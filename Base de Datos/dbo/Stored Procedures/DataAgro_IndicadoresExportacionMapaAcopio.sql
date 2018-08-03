CREATE PROCEDURE [dbo].[DataAgro_IndicadoresExportacionMapaAcopio]  

@ProvinciaId int   ,
@SegmentacionId VARCHAR(max) ,
@MaterialId int   ,
@CampañaId int ,   
@ComercialId int,
 @ComercialGenerador Int=null

as

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int);

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

declare @SegmentacionSecuencia TABLE (Item INT)    

insert into @SegmentacionSecuencia (Item) select Item  from dbo.Split (@SegmentacionId,',') ;
 
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialGenerador

select p.cuit,cm.toneladas as Toneladas,m.Descripcion as Material,c.Descripcion as Campaña,
prv.Nombre as Provincia,
case when seg.grupo ='Productores' then 'Productores ' + seg.Descripcion   else seg.Descripcion end as Segmentación
,emp.Apellido + ' ' + emp.Nombres as Comercial,p.razonSocial

from AcopioMaterial cm
inner join Acopio cp on cp.AcopioId=cm.AcopioId
inner join proveedor p on p.ProveedorId= cp.ProveedorId
inner join ProveedorComercial pc on pc.proveedorId= p.proveedorId
inner join @EmpleadoTable  emp on pc.ComercialId = emp.ComercialId
inner join segmentacion seg on p.segmentacionId=seg.segmentacionId
inner join Localidad loc on cp.LocalidadId=loc.LocalidadId
inner join Provincia prv on loc.ProvinciaId = prv.ProvinciaId 
inner join Material m on cm.MaterialId = m.MaterialId
inner join Campaña c on cm.CampañaId = c.CampañaId
where ( (@MaterialId is null) or (cm.MaterialId= @MaterialId))

and ( (@CampañaId is null) or (cm.CampañaId= @CampañaId))

and (( @SegmentacionId is null) or (@SegmentacionId= '0' and p.SegmentacionId is not null) 
	or (exists ( select 1 from @SegmentacionSecuencia where Item = p.SegmentacionId)))

and (( @ProvinciaId is null) 
	or (exists ( select 1 from @ProvinciaSecuencia where Item = loc.ProvinciaId)))

and ((@ComercialId is null) or ( pc.ComercialId = @ComercialId))

and loc.provinciaId is not null

ORDER BY P.CUIT
