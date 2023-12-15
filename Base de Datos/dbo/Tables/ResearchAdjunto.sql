CREATE TABLE [dbo].[ResearchAdjunto]
(
	[ResearchAdjuntoId] INT IDENTITY (1, 1) NOT NULL,
	[Path] VARCHAR (100) NULL,
	[Nombre] VARCHAR (50) NULL,
	[IdPowerApp] INT NULL,
	[Extension] VARCHAR (20) NULL,
	[Data] VARBINARY(MAX) NULL,
	CONSTRAINT [PK_ResearchAdjunto] PRIMARY KEY CLUSTERED ([ResearchAdjuntoId] ASC),
)
