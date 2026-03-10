CREATE TABLE [dbo].[AperturaPrecio]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [ConceptoAperturaPrecioId] INT NOT NULL,
	[Importe] DECIMAL(11, 2) NULL,
	[MonedaId] CHAR(5) NULL,
	[Porcentaje] DECIMAL(11, 2) NULL,
	[NegocioId] INT NOT NULL,
	CONSTRAINT [FK_AperturaPrecio_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [Negocio]([Id]),
	CONSTRAINT [FK_AperturaPrecio_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId])
)

GO
CREATE NONCLUSTERED INDEX NDX_AperturaPrecio_NegocioId_Porcentaje 
ON [dbo].[AperturaPrecio] ([ConceptoAperturaPrecioId],[NegocioId],[Porcentaje])

GO
CREATE NONCLUSTERED INDEX NDX_ConceptoAperturaPrecioId 
ON [dbo].[AperturaPrecio] ([ConceptoAperturaPrecioId]) 
INCLUDE ([Importe],[NegocioId])

GO
CREATE NONCLUSTERED INDEX NDX_NegocioId 
ON [dbo].[AperturaPrecio] ([NegocioId]) 
INCLUDE ([ConceptoAperturaPrecioId],[Importe])

GO
CREATE NONCLUSTERED INDEX [IX_AperturaPrecio_NegocioId_Concepto]
ON [dbo].[AperturaPrecio] ([NegocioId], [ConceptoAperturaPrecioId], [Porcentaje])
INCLUDE ([Importe])

GO