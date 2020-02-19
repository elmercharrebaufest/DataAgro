CREATE TABLE [dbo].[AperturaPrecio]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [ContratoId] INT NULL, 
	[FijacionId] INT NULL, 
    [ConceptoAperturaPrecioId] INT NOT NULL,
	[Importe] DECIMAL(11, 2) NULL,
	[MonedaId] CHAR(5) NULL,
	[Porcentaje] DECIMAL(11, 2) NULL,

	  CONSTRAINT [FK_AperturaPrecio_Contrato] FOREIGN KEY ([ContratoId]) REFERENCES [Contrato]([ContratoId]),
	  CONSTRAINT [FK_AperturaPrecio_FijacionId] FOREIGN KEY ([FijacionId]) REFERENCES [FijacionDePrecioContrato]([FijacionDePrecioContratoId]),
	  CONSTRAINT [FK_AperturaPrecio_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId])
)
