CREATE TABLE [dbo].[ResearchAdjunto]
(
	[ResearchAdjuntoId] INT IDENTITY (1, 1) NOT NULL,
	[Path] VARCHAR (2000) NULL,
	[Nombre] VARCHAR (500) NULL,
	[ResearchId] INT NULL,
	CONSTRAINT [PK_ResearchAdjunto] PRIMARY KEY CLUSTERED ([ResearchAdjuntoId] ASC),
	CONSTRAINT [FK_ResearchAdjunto_Research] FOREIGN KEY ([ResearchId]) REFERENCES [dbo].[Research] ([ResearchId]),
)
