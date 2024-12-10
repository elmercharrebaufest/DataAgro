 create procedure [dbo].[DataAgro_Contactos_Exportar] --'2,481,485,494,574,589,662,678,687,694,805,889,954',42  
 @Proveedores VARCHAR(max),  
 @ComercialId varchar(max)  
as  
  
  
declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)  
   
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId   
  
  
declare @table as table(item int )  
  
create table #ProveedorAux (ProveedorId int,CUIT varchar(20),RazonSocial varchar(100), Email1 varchar(255),   
Email2 varchar(255),Email3 varchar(255),Email4 varchar(255),Telefono1  varchar(255), Telefono2 varchar(255),  
Telefono3  varchar(255),Telefono4 varchar(255),ComercialAcargo varchar(100), FechaUltimoContacto datetime ,  
RiesgoComercialSap   varchar(255), Estado  varchar(255), Situacion  varchar(255),Faccop int, Calificacion int, GrupoDeCompras varchar(255), FechaAlta DATETIME, EstadoCuit  int)  
  
 insert into @table   
 select Item FROM dbo.Split(@Proveedores, ',')   
  
 insert into #ProveedorAux(ProveedorId,CUIT,RazonSocial,Email1,Email2,Email3,Email4,Telefono1,Telefono2,Telefono3  
 ,Telefono4,ComercialAcargo,FechaUltimoContacto,RiesgoComercialSap,Estado,Situacion,Faccop, Calificacion, GrupoDeCompras, FechaAlta,EstadoCuit)  
 select    
 p.ProveedorId,   
 p.CUIT as CUIT,   
 p.RazonSocial as RazonSocial,  
 cc.Email1 as Email1 ,  
 cc.Email2 as Email2 ,  
 cc.Email3 as Email3 ,  
 null as Email4 ,  
 cc.Telefono1 as Telefono1,   
 cc.Telefono2 as Telefono2,   
 cc.Telefono3 as Telefono3,  
 null as Telefono4,   
 (C.Nombres + ' ' + c.Apellido) as ComercialAcargo ,  
 p.FechaUltimoContacto as FechaUltimoContacto,  
 p.RiesgoComercialSap as RiesgoComercialSap,   
 NULL AS Estado,   
 (select top 1 EstadoCuit from SISA rg where rg.CUIT= p.cuit) as Situacion,(case when f.Id is null then 0 else 1 end) as Faccop,  
 p.Calificacion,  
 gdc.Descripcion as GrupoDecompras,  
 p.FechaAlta  ,
 isnull((select TOP 1 EstadoCuit from SISA where CUIT = p.CUIT),0) as EstadoCuit
  from Proveedor p  
 inner join ProveedorComercial pc on p.ProveedorId = pc.ProveedorId  
 inner join Comercial c on pc.ComercialId = c.ComercialId  
 left join FACACOP f on p.CUIT = f.CUIT  
 LEFT JOIN ContactoComercial CC ON CC.ProveedorId = p.ProveedorId and cc.EsPrincipal = 1  
 LEFT JOIN GrupoDeCompras gdc ON gdc.Id = c.GrupoDeComprasId  
 where p.ProveedorId in (select * from @table)  
 order by p.RazonSocial desc  
   
 update #ProveedorAux  
 set Estado = (select Descripcion from Estado where EstadoId =est.Estado )  
 from (  
 select   
distinct t.ProveedorId ,   
case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 4 and pee.ComercialId in (select ComercialId from  @EmpleadoTable))   
then 4 else   
    case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 5 and pee.ComercialId in (select ComercialId from  @EmpleadoTable))   
    then 5  else  
            case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 1 and pee.ComercialId in (select ComercialId from  @EmpleadoTable))   
            then 1  else  
                    case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 2 and pee.ComercialId in (select ComercialId from  @EmpleadoTable))   
                    then 2 else  
                        case when exists(select 1 from ProveedorEstado pee where t.ProveedorId = pee.ProveedorId and  pee.EstadoId = 3 and pee.ComercialId in (select ComercialId from  @EmpleadoTable))   
                        then 3  else  
                                (select EstadoId From Proveedor PP WHERE PP.ProveedorId = t.ProveedorId)  
                        ENd  
                    ENd  
            ENd  
    ENd  
ENd as Estado  
from #ProveedorAux t ) est  
where  est.ProveedorId = #ProveedorAux.ProveedorId  
  
select * from #ProveedorAux   
  
drop table #ProveedorAux  
--drop table @EmpleadoTable  