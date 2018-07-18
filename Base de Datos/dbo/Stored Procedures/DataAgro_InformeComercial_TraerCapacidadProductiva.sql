
CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_TraerCapacidadProductiva]
	@inf varchar(MAX)
as

declare @InformeSecuencia TABLE (Item INT)   
insert into @InformeSecuencia (Item) select Item  from dbo.Split (@inf,',')   

declare @Informes TABLE (proveedor BIgINT, TonMaiz BIgINT default(0) ,TonTrigo BIgINT default(0) ,TonSoja BIgINT default(0))   

insert into @Informes (proveedor)
select i.ProveedorId as ProveedorId
from informecomercial i
where  exists ( select 1 from @InformeSecuencia where Item = i.informeComercialId) 
group by i.ProveedorId

update @Informes
set TonMaiz = b.Toneladas
from 
(
select i.ProveedorId as ProveedorId, sum(Toneladas) Toneladas
from informecomercial i
inner join InformeComercialProduccion ip on i.InformeComercialId =ip.InformeComercialId 
where  ip.MaterialId = 1  and  exists ( select 1 from @InformeSecuencia where Item = i.informeComercialId) 
group by i.ProveedorId
) b
where proveedor= b.ProveedorId




update @Informes
set TonTrigo = b.Toneladas
from 
(
select i.ProveedorId as ProveedorId, sum(Toneladas) Toneladas
from informecomercial i
inner join InformeComercialProduccion ip on i.InformeComercialId =ip.InformeComercialId 
where  ip.MaterialId = 2  and  exists ( select 1 from @InformeSecuencia where Item = i.informeComercialId) 
group by i.ProveedorId
) b
where proveedor= b.ProveedorId


update @Informes
set TonSoja = b.Toneladas
from 
(
select i.ProveedorId as ProveedorId, sum(Toneladas) Toneladas
from informecomercial i
inner join InformeComercialProduccion ip on i.InformeComercialId =ip.InformeComercialId 
where  ip.MaterialId = 3  and  exists ( select 1 from @InformeSecuencia where Item = i.informeComercialId) 
group by i.ProveedorId
) b
where proveedor= b.ProveedorId

select P.cuit as proveedor
,cast(tonmaiz as decimal(18,2)) as Maiz
,cast(TonTrigo as decimal(18,2)) as Trigo
,cast(TonSoja as decimal(18,2)) as Soja  
from @Informes i
inner join proveedor p on p.proveedorID = I.proveedor