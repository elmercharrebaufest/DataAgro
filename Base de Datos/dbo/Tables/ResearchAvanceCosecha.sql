CREATE TABLE [dbo].[ResearchAvanceCosecha] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [RangoDesde] DECIMAL  NOT NULL DEFAULT(1),
	[RangoHasta] DECIMAL  NOT NULL  DEFAULT(1) ,
    [LocalidadId] INT           NOT NULL,
	[MaterialId] INT           NOT NULL,
	[ComercialId] INT           NOT NULL,
	[CampaniaId] INT  NOT NULL DEFAULT 7,
	[FechaHora] DATETIME NOT NULL,
    [Avance] DECIMAL (18,2) NOT NULL,
	[Rendimiento] DECIMAL (18,2) NOT NULL,
    [Observaciones] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_ResearchAvanceCosecha] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResearchAvanceCosecha_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
	CONSTRAINT [FK_ResearchAvanceCosecha_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_ResearchAvanceCosecha_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
	CONSTRAINT [FK_ResearchAvanceCosecha_Campaña] FOREIGN KEY ([CampaniaId]) REFERENCES [dbo].[Campaña] ([CampañaId])

);

