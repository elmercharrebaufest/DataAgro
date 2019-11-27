

CREATE PROCEDURE [dbo].[DataAgro_Comercial_TraerPorComerciales]

 @ComercialId NVARCHAR(MAX)

AS

declare @EmpleadoTable TABLE ( ComercialId int , Apellido varchar(255), Nombres varchar(255), PerfilId int, EmpleadorACargoId int , IdActiveDirectory varchar(255),GrupoDeComprasId int)
insert into @EmpleadoTable exec DataAgro_ComercialesJerarquicos_Traer @ComercialId

select ComercialId as Id, Apellido + ' ' + Nombres as Nombre
from @EmpleadoTable
order by Apellido asc, Nombres asc