CREATE TABLE [dbo].[ResearchAdjunto]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Path] VARCHAR (2000) NULL,
	[Nombre] VARCHAR (500) NULL,
	[ResearchId] INT NULL,
	CONSTRAINT [PK_ResearchAdjunto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_ResearchAdjunto_Research] FOREIGN KEY ([ResearchId]) REFERENCES [dbo].[Research] ([Id]),
)
