CREATE TABLE [dbo].[RangoPrecio] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [PrecioMaximo]          DECIMAL(18,2)	NOT NULL,
    [PrecioMinimo]         DECIMAL(18,2)	NOT NULL,
    [MaterialId]            INT				NOT NULL,
    [MonedaId]				CHAR(5)			NOT NULL
    CONSTRAINT [PK_RangoPrecio] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Calidad_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
	CONSTRAINT [FK_Calidad_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId])
);

