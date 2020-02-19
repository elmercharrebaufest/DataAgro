CREATE TABLE [dbo].[NotificacionResearch] (
    [Id] INT      IDENTITY (1, 1) NOT NULL,
    [CampanaId]                 INT      NOT NULL,
    [MaterialId]                INT      NOT NULL,
    [FechaDesde]                DATETIME NOT NULL,
    [FechaHasta]                DATETIME NOT NULL,
	[TipoResearchId]			INT NOT NULL,
	[Mensaje]					VARCHAR(MAX) NOT NULL
    CONSTRAINT [PK_NotificacionResearch] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NotificacionResearch_Campaña] FOREIGN KEY ([CampanaId]) REFERENCES [Campaña]([CampañaId]),
    CONSTRAINT [FK_NotificacionResearch_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
    CONSTRAINT [FK_NotificacionResearch_TipoResearch] FOREIGN KEY ([TipoResearchId]) REFERENCES [TipoResearch]([Id])

);

