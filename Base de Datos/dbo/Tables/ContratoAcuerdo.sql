CREATE TABLE [dbo].[ContratoAcuerdo]
(
	[Id] INT  IDENTITY (1, 1) NOT NULL,
    [ProveedorId] INT NOT NULL, 
    [Cantidad] INT NOT NULL, 
    [Precio] DECIMAL(11, 2) NOT NULL, 
    [MaterialId] INT NOT NULL, 
    [DestinoId] INT NOT NULL, 
    [FechaDesde] DATETIME2 NOT NULL, 
    [FechaHasta] DATETIME2 NOT NULL, 
    [Fecha] DATETIME2 NOT NULL, 
    [ComercialCreadorId] INT NOT NULL,
	[EstadoId] INT NOT NULL,
	[MonedaId] CHAR(5) NOT NULL,

    CONSTRAINT [FK_ContratoAcuerdo_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor]([ProveedorId]), 
	CONSTRAINT [FK_ContratoAcuerdo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
	CONSTRAINT [FK_ContratoAcuerdo_Destino] FOREIGN KEY ([DestinoId]) REFERENCES [Centro]([Id]),
	CONSTRAINT [FK_ContratoAcuerdo_ComercialCreador] FOREIGN KEY ([ComercialCreadorId]) REFERENCES [Comercial]([ComercialId]), 
    CONSTRAINT [PK_ContratoAcuerdo] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ContratoAcuerdo_EstadoContrato] FOREIGN KEY (EstadoId) REFERENCES [EstadoContrato]([EstadoContratoId]),
	CONSTRAINT [FK_ContratoAcuerdo_Moneda] FOREIGN KEY (MonedaId) REFERENCES [Moneda]([MonedaId])
)
