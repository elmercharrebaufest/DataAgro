CREATE TABLE [dbo].[PrecioPizarra] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [MaterialId] INT           NOT NULL,
    [PizarraId] INT           NOT NULL,
	[MonedaId]  CHAR(5)     NOT NULL,
    [FechaDesde] DATETIME2 NOT NULL,
	[FechaHasta] DATETIME2 NOT NULL,
    [Precio] INT NOT NULL,
	[UnidadMedida] VARCHAR (50) NULL DEFAULT 'TON',
    CONSTRAINT [PK_PrecioPizarra] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PrecioPizarra_Pizarra] FOREIGN KEY ([PizarraId]) REFERENCES [dbo].[Pizarra] ([Id]),
    CONSTRAINT [FK_PrecioPizarra_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].Material ([MaterialId]),
	CONSTRAINT [FK_PrecioPizarra_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [dbo].Moneda ([MonedaId])
);

