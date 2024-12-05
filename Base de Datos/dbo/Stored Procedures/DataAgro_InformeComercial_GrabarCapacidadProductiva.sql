
CREATE PROCEDURE [dbo].[DataAgro_InformeComercial_GrabarCapacidadProductiva]
	@inf varchar(MAX)
as

declare @InformeSecuencia TABLE (Item INT)   

insert into @InformeSecuencia (Item) select Item  from dbo.Split (@inf,',')   

update InformeComercial
set EstadoId= 2
where InformeComercialId in (select item from @InformeSecuencia)

select 1 as Res