CREATE TABLE [dbo].[ResearchEstadioFenologico]
(
	[EstadioFenologicoId] INT IDENTITY (1, 1) NOT NULL,
	[MaterialId] INT NOT NULL,
	[EstadioId] INT NOT NULL,
	[ConRendimiento]  BIT NOT NULL,
	CONSTRAINT [PK_ResearchEstadioFenologico] PRIMARY KEY CLUSTERED ([EstadioFenologicoId] ASC),
	CONSTRAINT [FK_ResearchEstadioFenologico_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_ResearchEstadioFenologico_ResearchEstadio] FOREIGN KEY ([EstadioId]) REFERENCES [dbo].[ResearchEstadio] ([EstadioId]),
)
