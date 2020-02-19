CREATE TABLE [dbo].[PrecioMoa] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [Precio]                DECIMAL(11, 2) NOT NULL, 
    [MonedaId]              CHAR(5) NOT NULL, 
    [MaterialId]            INT NOT NULL,
    [DesdeVigencia]		    DATETIME NOT NULL,
    [HastaVigencia]		    DATETIME NOT NULL,
    CONSTRAINT [PK_PrecioMoa] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PrecioMoa_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].Material ([MaterialId]),
	CONSTRAINT [FK_PrecioMoa_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [dbo].Moneda ([MonedaId])
);

