CREATE TABLE [dbo].[RangoConfirmacionAutomatica] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [PrecioMaximo]          DECIMAL(18,2)	NOT NULL,
    [PrecioMinimo]         DECIMAL(18,2)	NOT NULL,
    [MonedaId]				CHAR(5)			NOT NULL,
	[MaterialId]			INT NOT NULL,
    [FechaDesde] DATETIME NOT NULL DEFAULT 2019-02-14 , 

	
    CONSTRAINT [PK_RangoConfirmacionAutomatica] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Confirmacion_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId]),
	CONSTRAINT [FK_Confirmacion_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId])
);

