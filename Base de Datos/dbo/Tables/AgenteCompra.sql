CREATE TABLE [dbo].[AgenteCompra]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [MaterialId] INT NOT NULL ,  
    [Cantidad] FLOAT NOT NULL, 
    [Precio] DECIMAL(11, 2) NOT NULL, 
    [MonedaId] CHAR(5) NOT NULL, 
	[OperadorId] INT NOT NULL,
	[Posicion] NVARCHAR(10) NOT NULL,	
    [Fecha] DATETIME NOT NULL, 
    [ComercialId] INT NULL, 
	[EstadoId] INT NOT NULL,
	[Ampliaciones] FLOAT NULL
  
    CONSTRAINT [FK_AgenteCompra_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId]),  
    CONSTRAINT [FK_AgenteCompra_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
    CONSTRAINT [FK_AgenteCompra_Operador] FOREIGN KEY ([OperadorId]) REFERENCES [Operador]([Id]), 
    CONSTRAINT [FK_AgenteCompra_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId]),  
    CONSTRAINT [FK_AgenteCompra_EstadoContrato] FOREIGN KEY (EstadoId) REFERENCES [EstadoContrato]([EstadoContratoId])
)
