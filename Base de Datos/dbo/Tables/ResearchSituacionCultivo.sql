CREATE TABLE [dbo].[ResearchSituacionCultivo] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [LocalidadId] INT           NOT NULL,
	[MaterialId] INT           NOT NULL,
	[ComercialId] INT           NOT NULL,
	[CampaniaId] INT NOT NULL DEFAULT 7, 
	[FechaHora] DATETIME NOT NULL,
	[EstadioId] INT NULL,
	[Situacion] VARCHAR (50) NULL,
    [Observaciones] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_ResearchSituacionCultivo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResearchSituacionCultivo_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
	CONSTRAINT [FK_ResearchSituacionCultivo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_ResearchSituacionCultivo_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
	CONSTRAINT [FK_ResearchSituacionCultivo_Estadio] FOREIGN KEY ([EstadioId]) REFERENCES [dbo].[Estadio] ([Id]),	
	CONSTRAINT [FK_ResearchSituacionCultivo_Campaña] FOREIGN KEY ([CampaniaId]) REFERENCES [dbo].[Campaña] ([CampañaId])
);

