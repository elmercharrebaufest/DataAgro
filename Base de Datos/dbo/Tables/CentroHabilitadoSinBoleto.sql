CREATE TABLE [dbo].[CentroHabilitadoSinBoleto] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,	
    [CentroId]         INT        NOT NULL,
    [TipoNegocioId] INT NOT NULL, 
    CONSTRAINT [PK_CentroHabilitadoSinBoleto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CentroHabilitadoSinBoleto_Centro] FOREIGN KEY ([CentroId]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_CentroHabilitadoSinBoleto_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [dbo].[TipoNegocio] ([TiponegocioId])
);

