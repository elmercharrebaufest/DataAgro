CREATE TABLE [dbo].[Servicio]
(
	[Id] INT NOT NULL IDENTITY (1, 1), 
    [ServicioValorId] INT NOT NULL,
	[Importe] DECIMAL(11, 2) NULL,
	[MonedaId] CHAR(5) NULL,
	[NegocioId] INT NOT NULL,
	[Modificado] BIT NOT NULL , 
    [Desde] DECIMAL(11, 2) NULL, 
    [Hasta] DECIMAL(11, 2) NULL, 
	CONSTRAINT [PK_Servicio] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Servicio_ServicioValor] FOREIGN KEY ([ServicioValorId]) REFERENCES [ServicioValor]([Id]),
	CONSTRAINT [FK_Servicio_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId]),
	CONSTRAINT [FK_Servicio_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [Negocio]([Id]),
)

GO
CREATE NONCLUSTERED INDEX NDX_NegocioId 
ON [dbo].[Servicio] ([NegocioId])

GO