CREATE TABLE [dbo].[LimiteCupo] (
    [Id]					INT IDENTITY (1, 1) NOT NULL,
    [ConfiguracionCupoId]	INT					NOT NULL,
	[ZonaCupoId]			INT					NOT NULL, 
	[CantidadCupo]			INT					NOT NULL, 
    [LimiteAnterior] INT NULL, 
    [CantidadCupoConDescarga] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_LimiteCupo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_LimiteCupo_ConfiguracionCupo] FOREIGN KEY ([ConfiguracionCupoId]) REFERENCES [ConfiguracionCupo]([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_LimiteCupo_ZonaCupo] FOREIGN KEY ([ZonaCupoId]) REFERENCES [ZonaCupo]([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_LimiteCupo_Id]
    ON [dbo].[LimiteCupo]([ConfiguracionCupoId] ASC);