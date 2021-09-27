CREATE TABLE [dbo].[NegocioAsociado] (
    [Id]					INT IDENTITY (1, 1) NOT NULL,
    [AFijarId]			INT NOT NULL,
    [AsociadoId]			INT NOT NULL,
    CONSTRAINT [PK_NegocioAsociado] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NegocioAsociado_AFijar] FOREIGN KEY ([AFijarId]) REFERENCES [dbo].[Negocio] ([Id]),
    CONSTRAINT [FK_NegocioAsociado_Asociado] FOREIGN KEY ([AsociadoId]) REFERENCES [dbo].[Negocio] ([Id])
);