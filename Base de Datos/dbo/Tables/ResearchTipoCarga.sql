CREATE TABLE [dbo].[ResearchTipoCarga]
(
	[TipoCargaId] INT IDENTITY (1, 1) NOT NULL,
	[Descripcion] VARCHAR (50) NULL,
	CONSTRAINT [PK_ResearchTipoCarga] PRIMARY KEY CLUSTERED ([TipoCargaId] ASC),
)
