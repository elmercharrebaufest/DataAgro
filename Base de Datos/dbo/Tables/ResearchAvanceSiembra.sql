CREATE TABLE [dbo].[ResearchAvanceSiembra] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [IntencionSiembra] DECIMAL (18,2)   NOT NULL,
    [LocalidadId] INT           NOT NULL,
	[MaterialId] INT           NOT NULL,
	[ComercialId] INT           NOT NULL,
	[CampaniaId] INT NOT NULL DEFAULT 7,
	[FechaHora] DATETIME NOT NULL,
    [Avance] DECIMAL (18,2) NOT NULL,
	[CambioAA] DECIMAL (18,2) NOT NULL,
    [Observaciones] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_ResearchAvanceSiembra] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResearchAvanceSiembra_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
	CONSTRAINT [FK_ResearchAvanceSiembra_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_ResearchAvanceSiembra_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
	CONSTRAINT [FK_ResearchAvanceSiembra_Campaña] FOREIGN KEY ([CampaniaId]) REFERENCES [dbo].[Campaña] ([CampañaId])
);

