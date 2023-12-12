CREATE TABLE [dbo].[ResearchTipoMuestra]
(
	[TipoMuestraId] INT IDENTITY (1, 1) NOT NULL,
	[Descripcion] VARCHAR (50) NULL,
	CONSTRAINT [PK_ResearchTipoMuestra] PRIMARY KEY CLUSTERED ([TipoMuestraId] ASC),
)
