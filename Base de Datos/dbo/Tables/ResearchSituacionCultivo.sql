CREATE TABLE [dbo].[ResearchSituacionCultivo] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [LocalidadId] INT           NOT NULL,
	[MaterialId] INT           NOT NULL,
	[ComercialId] INT           NOT NULL,
	[FechaHora] DATETIME NOT NULL,
	[Estadio] VARCHAR (50) NULL,
	[Situacion] VARCHAR (50) NULL,
    [Observaciones] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_ResearchSituacionCultivo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResearchSituacionCultivo_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
	CONSTRAINT [FK_ResearchSituacionCultivo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_ResearchSituacionCultivo_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId])
);

