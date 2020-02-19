CREATE TABLE [dbo].[ConfiguracionCupo] (
    [Id]					INT IDENTITY (1, 1) NOT NULL,
    [MaterialId]			INT					NOT NULL,
	[Fecha]					DATETIME			NOT NULL, 
	[LimiteCupo]			INT					NOT NULL, 
	[CentroId]				INT					NOT NULL, 
    CONSTRAINT [PK_ConfiguracionCupo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_ConfiguracionCupo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
	CONSTRAINT [FK_ConfiguracionCupo_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id])
);