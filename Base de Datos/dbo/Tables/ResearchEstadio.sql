CREATE TABLE [dbo].[ResearchEstadio]
(
	[EstadioId] INT IDENTITY (1, 1) NOT NULL,
	[Descripcion] VARCHAR (50) NULL,
	CONSTRAINT [PK_ResearchEstadio] PRIMARY KEY CLUSTERED ([EstadioId] ASC),
)
