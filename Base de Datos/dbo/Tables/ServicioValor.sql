CREATE TABLE [dbo].[ServicioValor]
(
	[Id] INT NOT NULL IDENTITY (1, 1), 
    [TipoServicioId] INT NOT NULL,
	[Importe] DECIMAL(11, 2) NULL,
	[MonedaId] CHAR(5) NULL,
	[MaterialId] INT NOT NULL,
	 [Desde] DECIMAL(11, 2) NULL, 
    [Hasta] DECIMAL(11, 2) NULL, 
	[CentroId] INT NOT NULL,
	CONSTRAINT [PK_ServicioValor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ServicioValor_TipoServicio] FOREIGN KEY ([TipoServicioId]) REFERENCES [TipoServicio]([Id]),
	 CONSTRAINT [FK_ServicioValor_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId]),
	  CONSTRAINT [FK_ServicioValor_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
	    CONSTRAINT [FK_ServicioValor_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id]),
)
