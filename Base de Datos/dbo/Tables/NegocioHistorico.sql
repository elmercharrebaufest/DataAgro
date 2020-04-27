CREATE TABLE [dbo].[NegocioHistorico]
(
	[Id]					INT IDENTITY (1, 1) NOT NULL,
    [NegocioId] INT NOT NULL, 
    [TipoNegocioId] INT NOT NULL, 
    [Datos] varchar(max) NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [ComercialId] INT NULL, 
    CONSTRAINT [PK_NegocioHistorico] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_NegocioHistorico_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [dbo].[Negocio] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NegocioHistorico_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [TipoNegocio]([TipoNegocioId]),
    CONSTRAINT [FK_NegocioHistorico_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
)
