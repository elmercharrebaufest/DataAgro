CREATE TABLE [dbo].[HabilitacionCupo]
(
	[Id]					INT NOT NULL IDENTITY (1, 1),     
	[ZonaCupoId]			INT NOT NULL,
    [MaterialId]			INT NOT NULL,
    [FechaHasta]		DATETIME NOT NULL, 
    [FechaDesde]		DATETIME NOT NULL, 
    CONSTRAINT [PK_HabilitacionCupo] PRIMARY KEY CLUSTERED ([Id] ASC),	
    CONSTRAINT [FK_HabilitacionCupo_ZonaCupo] FOREIGN KEY (ZonaCupoId) REFERENCES [ZonaCupo]([Id]),
    CONSTRAINT [FK_HabilitacionCupo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
   
	
)