CREATE TABLE [dbo].[TipoNegocioHabilitadoSinBoleto] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,	
    [TipoNegocioId]         INT        NOT NULL,
    CONSTRAINT [PK_TipoNegocioHabilitadoSinBoleto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TipoNegocioHabilitadoSinBoleto_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [dbo].[TipoNegocio] ([TipoNegocioId])
);

