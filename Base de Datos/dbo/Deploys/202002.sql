SET XACT_ABORT ON

begin transaction

CREATE TABLE [dbo].[Negocio]
(
	[Id] INT  IDENTITY (1, 1) NOT NULL,
	[ContratoId] INT  NULL , 
    [MaterialId] INT NOT NULL , 
    [TipoNegocioId] INT NOT NULL DEFAULT 1, 
    [Cantidad] FLOAT NULL, 
    [Precio] DECIMAL(11, 2) NULL, 
    [FechaEntrega] DATETIME NULL, 
    [CampanaId] INT NULL, 
    [FechaDesde] DATETIME NULL, 
    [FechaHasta] DATETIME NULL, 
    [ProveedorId] INT NULL, 
    [MonedaId] CHAR(5) NULL, 
    [Fecha] DATETIME NULL, 
    [GrupoCompra] INT NULL, 
    [ComercialId] INT NULL, 
	[ProvinciaId] INT NULL,
    [LocalidadId] INT NULL, 
    [Base] BIT NULL, 
    ImporteSustentable DECIMAL(11, 2) NULL, 
	MonedaSustentableId CHAR(5) NULL, 
	FechaDolarizado DATETIME NULL,  
	DiasPesificado INT NULL,  
    [NoInformaSIO] BIT NULL, 
    [TrigoEspecial] BIT NULL, 
    [EstadoId] INT NOT NULL, 
    [UsuarioId] VARCHAR(100) NULL,
    [ContratoSAP] NVARCHAR(15) NULL, 
    [Ampliaciones] FLOAT NULL, 
	[ClasificacionId] INT DEFAULT 1 NOT NULL,  
    [Observacion] NVARCHAR(MAX) NULL,
	DestinoId INT DEFAULT 1 NULL,
	CantidadCamiones INT NULL,
	Consignatario BIT NULL,
	PlanCanje BIT NULL,
	CondicionFijacionId INT NULL,
	CD BIT NULL,
	Warrant BIT NULL,
	PagoDirectoVendedor BIT NULL,
	EstablecimientoPropio BIT NULL,
	[BoletoId] INT DEFAULT 3 NOT NULL,  
	[BolsaId] INT NULL,
	[DesdeFijacion] DATETIME NULL,
	[HastaFijacion] DATETIME NULL,
	[MercsDeposito] BIT NULL,
    [ComercialCreadorId] INT NULL,
	[CorredorId] INT NULL,
	[PorcentajeComision] DECIMAL(11, 2) NULL,
	[ContratoVendedor] NVARCHAR(15) NULL,
    [ContratoCorredor] NVARCHAR(15) NULL,
	[SelCargoVendedor] BIT NULL,
	[SelCargoMOA] BIT NULL,
	[Madre] BIT NULL,
	[ContratoMadre] NVARCHAR(15) NULL,
	[FinDelDiaId] INT NULL,
	[ContratoAcuerdoId] INT NULL,

    [Pizarra] BIT NULL DEFAULT 0, 
    [PrecioNeto] DECIMAL(11, 2) NULL, 
    [StandardDeCalidadId] INT NULL, 
	[PagoDiferido] BIT NULL,
	[ZonaId] INT NULL,
	[NivelTarifaId] INT NULL,
	[TarifaFlete] DECIMAL(11,2) NULL,
	[Compensacion] BIT NULL,
    [Dolarizado] BIT NULL, 
    [Sustentable] BIT NULL, 

	[OperadorId] INT NULL,
	[TipoAgenteCompraId] INT NULL,
	[Posicion] NVARCHAR(10) NULL,	

	[FijacionSAP] NVARCHAR(15) NULL,
	[PagoDiferidoContrato] BIT NULL,
    [ProveedorCreadorId] INT NULL, 
    [MotivoRechazo] NVARCHAR(1000) NULL, 
	[Discriminator]          VARCHAR(250)           NOT NULL,
	[TipoFasonId] [int] NULL,
		[idAnterior] INT NULL, 
	CONSTRAINT [PK_dbo.Negocio] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Negocio_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [TipoNegocio]([TipoNegocioId]),  
    CONSTRAINT [FK_Negocio_Campaña] FOREIGN KEY ([CampanaId]) REFERENCES [Campaña]([CampañaId]), 
    CONSTRAINT [FK_Negocio_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor]([ProveedorId]), 
    CONSTRAINT [FK_Negocio_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId]), 
    CONSTRAINT [FK_Negocio_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId]), 
    CONSTRAINT [FK_Negocio_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [Localidad]([LocalidadId]), 
    CONSTRAINT [FK_Negocio_EstadoContrato] FOREIGN KEY (EstadoId) REFERENCES [EstadoContrato]([EstadoContratoId]),
    CONSTRAINT [FK_Negocio_Es_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [Localidad]([LocalidadId]), 
	CONSTRAINT [FK_Negocio_ClasificacionCompraNet] FOREIGN KEY ([ClasificacionId]) REFERENCES [ClasificacionCompraNet]([Id]), 
	CONSTRAINT [FK_Negocio_BoletoCompraNet] FOREIGN KEY ([BoletoId]) REFERENCES [BoletoCompraNet]([Id]),	
	CONSTRAINT [FK_Negocio_BolsaCompraNet] FOREIGN KEY ([BolsaId]) REFERENCES [BolsaCompraNet]([Id]),
    CONSTRAINT [FK_Negocio_Destino] FOREIGN KEY ([DestinoId]) REFERENCES [Centro](Id),
    CONSTRAINT [FK_Negocio_CondicionFijacion] FOREIGN KEY ([CondicionFijacionId]) REFERENCES [CondicionFijacion]([Id]), 
    CONSTRAINT [FK_Negocio_ComercialCreador] FOREIGN KEY ([ComercialCreadorId]) REFERENCES [Comercial]([ComercialId]), 
    CONSTRAINT [FK_Negocio_Corredor] FOREIGN KEY ([CorredorId]) REFERENCES [Proveedor]([ProveedorId]),
	CONSTRAINT [FK_Negocio_FinDelDia] FOREIGN KEY (FinDelDiaId) REFERENCES [FinDelDia]([Id]),
	CONSTRAINT [FK_Negocio_ContratoAcuerdo] FOREIGN KEY (ContratoAcuerdoId) REFERENCES [ContratoAcuerdo]([Id]),
	CONSTRAINT [FK_Negocio_StandardDeCalidad] FOREIGN KEY (StandardDeCalidadId) REFERENCES [StandardDeCalidad]([Id]),
	CONSTRAINT [FK_Negocio_Zona] FOREIGN KEY (ZonaId) REFERENCES [Zona]([Id]),
	CONSTRAINT [FK_Negocio_NivelTarifa] FOREIGN KEY (NivelTarifaId) REFERENCES [NivelTarifa]([Id]),
	CONSTRAINT [FK_Negocio_TipoFason] FOREIGN KEY([TipoFasonId]) REFERENCES [dbo].[TipoFason] ([Id])

)

GO
CREATE NONCLUSTERED INDEX IX_Fecha_ComercialId_EstadoId
ON [dbo].[Negocio] ([Fecha],[ComercialId],[EstadoId])
INCLUDE ([ContratoId],[Cantidad],[Precio],[ProveedorId],[MonedaId],[ComercialCreadorId])
GO

--insert de contrato
INSERT INTO Negocio (Discriminator,idAnterior,MaterialId,TipoNegocioId,Cantidad,Precio,FechaEntrega,CampanaId,FechaDesde,FechaHasta,ProveedorId,MonedaId,Fecha,GrupoCompra,ComercialId,
ProvinciaId,LocalidadId,Base,ImporteSustentable,MonedaSustentableId,FechaDolarizado,DiasPesificado,NoInformaSIO,TrigoEspecial,EstadoId,UsuarioId,ContratoSAP,Ampliaciones,
ClasificacionId,Observacion,DestinoId,CantidadCamiones,Consignatario,PlanCanje,CondicionFijacionId,CD,Warrant,PagoDirectoVendedor,EstablecimientoPropio,BoletoId,BolsaId,
DesdeFijacion,HastaFijacion,MercsDeposito,ComercialCreadorId,CorredorId,PorcentajeComision,ContratoVendedor,ContratoCorredor,SelCargoVendedor,SelCargoMOA,Madre,ContratoMadre,
FinDelDiaId,ContratoAcuerdoId,Pizarra,PrecioNeto,StandardDeCalidadId,PagoDiferido,ZonaId,NivelTarifaId,TarifaFlete,Compensacion,Dolarizado,Sustentable)
select	'Contrato',ContratoId,MaterialId,TipoNegocioId,Cantidad,Precio,FechaEntrega,CampanaId,FechaDesde,FechaHasta,ProveedorId,MonedaId,Fecha,GrupoCompra,ComercialId,
ProvinciaId,LocalidadId,Base,ImporteSustentable,MonedaSustentableId,FechaDolarizado,DiasPesificado,NoInformaSIO,TrigoEspecial,EstadoId,UsuarioId,ContratoSAP,Ampliaciones,
ClasificacionId,Observacion,DestinoId,CantidadCamiones,Consignatario,PlanCanje,CondicionFijacionId,CD,Warrant,PagoDirectoVendedor,EstablecimientoPropio,BoletoId,BolsaId,
DesdeFijacion,HastaFijacion,MercsDeposito,ComercialCreadorId,CorredorId,PorcentajeComision,ContratoVendedor,ContratoCorredor,SelCargoVendedor,SelCargoMOA,Madre,ContratoMadre,
FinDelDiaId,ContratoAcuerdoId,Pizarra,PrecioNeto,StandardDeCalidadId,PagoDiferido,ZonaId,NivelTarifaId,TarifaFlete,Compensacion,Dolarizado,Sustentable
from Contrato

--insert de FijacionDePrecioContrato
INSERT INTO Negocio (Discriminator,TipoNegocioId,idAnterior,ContratoId,ProveedorId,MaterialId,MonedaId,ComercialId,Precio,Cantidad,Fecha,Ampliaciones,EstadoId,Observacion
,ComercialCreadorId,CorredorId,FijacionSAP,ContratoSAP,FechaDesde,FechaHasta,CampanaId,Posicion,TrigoEspecial,FinDelDiaId,Pizarra,PagoDiferidoContrato,DiasPesificado,PagoDiferido
,PrecioNeto,DestinoId,ProveedorCreadorId,MotivoRechazo,UsuarioId)
select	'FijacionDePrecioContrato',3,FijacionDePrecioContratoId,ContratoId,ProveedorId,MaterialId,MonedaId,ComercialId,Precio,Cantidad,Fecha,Ampliaciones,EstadoId,Observacion
,ComercialCreadorId,CorredorId,FijacionSAP,ContratoSAP,FechaDesde,FechaHasta,CampanaId,Posicion,TrigoEspecial,FinDelDiaId,Pizarra,PagoDiferidoContrato,DiasPesificado,PagoDiferido
,PrecioNeto,DestinoId,ProveedorCreadorId,MotivoRechazo,UsuarioCreador
from FijacionDePrecioContrato

--insert de Fason
INSERT INTO Negocio (Discriminator,TipoNegocioId,idAnterior,TipoFasonId,ProveedorId,MaterialId,CampanaId,Cantidad,Precio,MonedaId,Posicion,Fecha,ComercialId,EstadoId,
Ampliaciones,FechaDesde,FechaHasta,TrigoEspecial,FinDelDiaId,ComercialCreadorId)
select						'Fason',4,			Id,			TipoFasonId,FasoneroId ,MaterialId,CampanaId,Cantidad,Precio,MonedaId,Posicion,Fecha,ComercialId,EstadoId,
Ampliaciones,FechaDesde,FechaHasta,Especial,FinDelDiaId,ComercialCreadorId
from Fason

--insert de AgenteCompra
INSERT INTO Negocio (Discriminator,TipoNegocioId,idAnterior,MaterialId,Cantidad,Precio,MonedaId,OperadorId,Posicion,Fecha,ComercialId,EstadoId,Ampliaciones,TipoAgenteCompraId
,ComercialCreadorId,CampanaId)
select						'AgenteCompra',5,			Id,MaterialId,Cantidad,Precio,MonedaId,OperadorId,Posicion,Fecha,ComercialId,EstadoId,Ampliaciones,TipoAgenteCompraId
,ComercialCreadorId,CampanaId
from AgenteCompra

--insert de ContratoAcuerdo
INSERT INTO Negocio (Discriminator,TipoNegocioId,idAnterior,ProveedorId,Cantidad,Precio,MaterialId,DestinoId,FechaDesde,FechaHasta,Fecha,ComercialCreadorId,EstadoId,MonedaId
,CorredorId,StandardDeCalidadId)
select						'ContratoAcuerdo',6,			Id,ProveedorId,Cantidad,Precio,MaterialId,DestinoId,FechaDesde,FechaHasta,Fecha,ComercialCreadorId,EstadoId,MonedaId
,CorredorId,StandardDeCalidadId
from ContratoAcuerdo


go
--Contrato relaciones
--DataAgro.dbo.AperturaPrecio: FK_AperturaPrecio_Contrato
ALTER TABLE [dbo].[AperturaPrecio]  drop constraint FK_AperturaPrecio_Contrato
go
update AperturaPrecio set ContratoId = negocio.id
from AperturaPrecio
inner join Negocio on AperturaPrecio.ContratoId =Negocio.IdAnterior
where Negocio.Discriminator = 'Contrato' 

go
ALTER TABLE [dbo].[AperturaPrecio]  WITH CHECK ADD  CONSTRAINT [FK_AperturaPrecio_Negocio] FOREIGN KEY([ContratoId])
REFERENCES [dbo].[Negocio] ([Id])
GO

--DataAgro.dbo.Cupo: FK_Cupo_Contrato
alter table Cupo drop constraint [FK_Cupo_Contrato]
go
update Cupo set ContratoId = n.id
from Cupo c
inner join Negocio n on c.ContratoId =n.IdAnterior
where n.Discriminator = 'Contrato' 
go
ALTER TABLE [dbo].[Cupo]  WITH NOCHECK ADD  CONSTRAINT [FK_Cupo_Contrato] FOREIGN KEY([ContratoId])
REFERENCES [dbo].[Negocio] ([Id])
GO
--DataAgro.dbo.DescuentoBonificacion: FK_dbo.DescuentoBonificacion_Contrato
alter table DescuentoBonificacion drop constraint [FK_dbo.DescuentoBonificacion_Contrato]
go
update DescuentoBonificacion set ContratoId = n.id
from DescuentoBonificacion c
inner join Negocio n on c.ContratoId =n.IdAnterior
where n.Discriminator = 'Contrato' 
go
ALTER TABLE [dbo].[DescuentoBonificacion]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DescuentoBonificacion_Contrato] FOREIGN KEY([ContratoId])
REFERENCES [dbo].[Negocio] ([Id])
ON DELETE CASCADE
GO

--DataAgro.dbo.FijacionDePrecioContrato: FK_FijacionDePrecioContrato_Contrato
--no existe mas ahor es 
update c set ContratoId = a.id
from Negocio c
inner join Negocio a on  a.IdAnterior = c.ContratoId
where c.Discriminator = 'FijacionDePrecioContrato' and c.ContratoId is not null 
go
alter table negocio WITH CHECK add CONSTRAINT [FK_FijacionDePrecioContrato_Contrato_New] FOREIGN KEY ([ContratoId]) REFERENCES [Negocio]([Id])
--DataAgro.dbo.DescuentoBonificacion: FK_dbo.DescuentoBonificacion_Contrato
alter table DescuentoBonificacion drop constraint [FK_dbo.DescuentoBonificacion_Contrato]
go
update DescuentoBonificacion set ContratoId = n.id
from DescuentoBonificacion c
inner join Negocio n on c.ContratoId =n.IdAnterior
where n.Discriminator = 'Contrato' 
go
ALTER TABLE DescuentoBonificacion  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.DescuentoBonificacion_Contrato] FOREIGN KEY([ContratoId])
REFERENCES [dbo].[Negocio] ([Id])
go
--DataAgro.dbo.SugerenciaCupo: FK_SugerenciaCupo_Contrato
alter table SugerenciaCupo drop constraint [FK_SugerenciaCupo_Contrato]
go
update SugerenciaCupo set ContratoId = n.id
from SugerenciaCupo c
inner join Negocio n on c.ContratoId =n.IdAnterior
where n.Discriminator = 'Contrato' 
go
ALTER TABLE SugerenciaCupo  WITH NOCHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Contrato] FOREIGN KEY([ContratoId])
REFERENCES [dbo].[Negocio] ([Id])
go


-- FijacionDePrecioContrato Relacione
--DataAgro.dbo.AperturaPrecio: FK_AperturaPrecio_FijacionId
alter table AperturaPrecio drop constraint [FK_AperturaPrecio_FijacionId]
go
update AperturaPrecio set FijacionId = n.id
from AperturaPrecio c
inner join Negocio n on c.FijacionId =n.IdAnterior
where n.Discriminator = 'FijacionDePrecioContrato' 
go
ALTER TABLE AperturaPrecio  WITH NOCHECK ADD  CONSTRAINT [FK_AperturaPrecio_FijacionId] FOREIGN KEY([FijacionId])
REFERENCES [dbo].[Negocio] ([Id])
go
--DataAgro.dbo.Cupo: FK_Cupo_FijacionDePrecioContrato
alter table Cupo drop constraint [FK_Cupo_FijacionDePrecioContrato]
go
update Cupo set FijacionDePrecioContratoId = n.id
from Cupo c
inner join Negocio n on c.FijacionDePrecioContratoId =n.IdAnterior
where n.Discriminator = 'FijacionDePrecioContrato' 
go
ALTER TABLE Cupo  WITH NOCHECK ADD  CONSTRAINT [FK_Cupo_FijacionDePrecioContrato] FOREIGN KEY([FijacionDePrecioContratoId])
REFERENCES [dbo].[Negocio] ([Id])
go
--DataAgro.dbo.SugerenciaCupo: FK_SugerenciaCupo_FijacionDePrecioContrato
alter table SugerenciaCupo drop constraint [FK_SugerenciaCupo_FijacionDePrecioContrato]
go
update SugerenciaCupo set FijacionDePrecioContratoId = n.id
from SugerenciaCupo c
inner join Negocio n on c.FijacionDePrecioContratoId =n.IdAnterior
where n.Discriminator = 'FijacionDePrecioContrato' 
go
ALTER TABLE SugerenciaCupo  WITH NOCHECK ADD  CONSTRAINT [FK_SugerenciaCupo_FijacionDePrecioContrato] FOREIGN KEY([FijacionDePrecioContratoId])
REFERENCES [dbo].[Negocio] ([Id])
go

--Relaciones Fason
--DataAgro.dbo.Cupo: FK_Cupo_Fason
alter table Cupo drop constraint [FK_Cupo_Fason]
go
update Cupo set FasonId = n.id
from Cupo c
inner join Negocio n on c.FasonId =n.IdAnterior
where n.Discriminator = 'Fason' 
go
ALTER TABLE Cupo  WITH NOCHECK ADD  CONSTRAINT [FK_Cupo_Fason] FOREIGN KEY([FasonId])
REFERENCES [dbo].[Negocio] ([Id])
go
--DataAgro.dbo.SugerenciaCupo: FK_SugerenciaCupo_Fason
alter table SugerenciaCupo drop constraint [FK_SugerenciaCupo_Fason]
go
update SugerenciaCupo set FasonId = n.id
from SugerenciaCupo c
inner join Negocio n on c.FasonId =n.IdAnterior
where n.Discriminator = 'Fason' 
go
ALTER TABLE SugerenciaCupo  WITH NOCHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Fason] FOREIGN KEY([FasonId])
REFERENCES [dbo].[Negocio] ([Id])
go

--Relaciones AgenteCompra
--DataAgro.dbo.Cupo: FK_Cupo_AgenteCompra
alter table Cupo drop constraint [FK_Cupo_AgenteCompra]
go
update Cupo set AgenteCompraId = n.id
from Cupo c
inner join Negocio n on c.AgenteCompraId =n.IdAnterior
where n.Discriminator = 'AgenteCompra' 
go
ALTER TABLE Cupo  WITH NOCHECK ADD  CONSTRAINT [FK_Cupo_AgenteCompra] FOREIGN KEY([AgenteCompraId])
REFERENCES [dbo].[Negocio] ([Id])
go
--DataAgro.dbo.SugerenciaCupo: FK_SugerenciaCupo_AgenteCompra
alter table SugerenciaCupo drop constraint [FK_SugerenciaCupo_AgenteCompra]
go
update SugerenciaCupo set AgenteCompraId = n.id
from SugerenciaCupo c
inner join Negocio n on c.AgenteCompraId =n.IdAnterior
where n.Discriminator = 'AgenteCompra' 
go
ALTER TABLE SugerenciaCupo  WITH NOCHECK ADD  CONSTRAINT [FK_SugerenciaCupo_AgenteCompra] FOREIGN KEY([AgenteCompraId])
REFERENCES [dbo].[Negocio] ([Id])
go

--Relaciones ContratoAcuerdo
--DataAgro.dbo.Calidad: FK_Calidad_Acuerdo
alter table Calidad drop constraint FK_Calidad_Acuerdo
go
alter table Calidad drop constraint FK_Calidad_Contrato
go
update Calidad set AcuerdoId = n.id
from Calidad c
inner join Negocio n on c.AcuerdoId =n.IdAnterior
where n.Discriminator = 'ContratoAcuerdo' 
go

update Calidad set ContratoId = n.id
from Calidad c
inner join Negocio n on c.ContratoId =n.IdAnterior
where n.Discriminator = 'Contrato' 
go

--Calidad
go
alter table Calidad add NegocioId int  null 
go
update calidad set NegocioId = ContratoId where ContratoId is not null
update calidad set NegocioId = AcuerdoId where AcuerdoId is not null
go
ALTER TABLE [dbo].[Calidad]  WITH CHECK ADD  CONSTRAINT [FK_Calidad_Negocio] FOREIGN KEY([NegocioId])
REFERENCES [dbo].[Negocio] ([Id])
ON DELETE CASCADE
go
alter table calidad drop column contratoId
go
alter table calidad drop column acuerdoId
go
alter table Calidad alter column NegocioId int not null 
go

--AperturaPrecio
go
alter table AperturaPrecio drop constraint FK_AperturaPrecio_Negocio
go
alter table AperturaPrecio drop constraint FK_AperturaPrecio_FijacionId
go
alter table AperturaPrecio add NegocioId int  null 
go
update AperturaPrecio set NegocioId = ContratoId where ContratoId is not null
update AperturaPrecio set NegocioId = FijacionId where FijacionId is not null
go
ALTER TABLE [dbo].[AperturaPrecio]  WITH CHECK ADD  CONSTRAINT [FK_AperturaPrecio_Negocio] FOREIGN KEY([NegocioId])
REFERENCES [dbo].[Negocio] ([Id])
ON DELETE CASCADE
go
alter table AperturaPrecio drop column contratoId
go
alter table AperturaPrecio drop column FijacionId
go
alter table AperturaPrecio alter column NegocioId int not null 
go

--SugerenciaCupo
go
alter table SugerenciaCupo drop constraint FK_SugerenciaCupo_AgenteCompra
go
alter table SugerenciaCupo drop constraint FK_SugerenciaCupo_Contrato
go
alter table SugerenciaCupo drop constraint FK_SugerenciaCupo_Fason
go
alter table SugerenciaCupo drop constraint FK_SugerenciaCupo_FijacionDePrecioContrato
go
alter table SugerenciaCupo add NegocioId int  null 
go
update SugerenciaCupo set NegocioId = ContratoId where ContratoId is not null
update SugerenciaCupo set NegocioId = FasonId where FasonId is not null
update SugerenciaCupo set NegocioId = AgenteCompraId where AgenteCompraId is not null
update SugerenciaCupo set NegocioId = FijacionDePrecioContratoId where FijacionDePrecioContratoId is not null
go
ALTER TABLE SugerenciaCupo  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Negocio] FOREIGN KEY([NegocioId])
REFERENCES [dbo].[Negocio] ([Id])
ON DELETE CASCADE
go
alter table SugerenciaCupo drop column FasonId
go
alter table SugerenciaCupo drop column ContratoId
go
alter table SugerenciaCupo drop column AgenteCompraId
go
alter table SugerenciaCupo drop column FijacionDePrecioContratoId
go

--Cupo
go
alter table Cupo drop constraint FK_Cupo_AgenteCompra
go
alter table Cupo drop constraint FK_Cupo_Contrato
go
alter table Cupo drop constraint FK_Cupo_Fason
go
alter table Cupo drop constraint FK_Cupo_FijacionDePrecioContrato
go
alter table Cupo add NegocioId int  null 
go
update Cupo set NegocioId = ContratoId where ContratoId is not null
update Cupo set NegocioId = FasonId where FasonId is not null
update Cupo set NegocioId = AgenteCompraId where AgenteCompraId is not null
update Cupo set NegocioId = FijacionDePrecioContratoId where FijacionDePrecioContratoId is not null
go
ALTER TABLE Cupo  WITH CHECK ADD  CONSTRAINT [FK_Cupo_Negocio] FOREIGN KEY([NegocioId])
REFERENCES [dbo].[Negocio] ([Id])
ON DELETE CASCADE
go
alter table Cupo drop column FasonId
go
alter table Cupo drop column ContratoId
go
alter table Cupo drop column AgenteCompraId
go
alter table Cupo drop column FijacionDePrecioContratoId
go

alter table Negocio drop column idanterior
go

update negocio set fechadesde = '1753-01-01' where fechadesde is null
update negocio set FechaHasta = '1753-01-01' where FechaHasta is null


  go
commit transaction