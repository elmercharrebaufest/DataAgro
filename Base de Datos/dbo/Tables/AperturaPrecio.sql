CREATE TABLE [dbo].[AperturaPrecio]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [ConceptoAperturaPrecioId] INT NOT NULL,
	[Importe] DECIMAL(11, 2) NULL,
	[MonedaId] CHAR(5) NULL,
	[Porcentaje] DECIMAL(11, 2) NULL,

	  [NegocioId] INT NOT NULL , 
    CONSTRAINT [FK_AperturaPrecio_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [Negocio]([Id]),
	  CONSTRAINT [FK_AperturaPrecio_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId])
)
