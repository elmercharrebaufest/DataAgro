CREATE TABLE [dbo].[ResearchCondicionCultivo]
(
	[CondicionCultivoId] INT IDENTITY (1, 1) NOT NULL,
	[MaterialId] INT NOT NULL,
	[CondicionId] INT NOT NULL,
	[Valor] INT NOT NULL,
	CONSTRAINT [PK_ResearchCondicionCultivo] PRIMARY KEY CLUSTERED ([CondicionCultivoId] ASC),
	CONSTRAINT [FK_ResearchCondicionCultivo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_ResearchCondicionCultivo_ResearchCondicion] FOREIGN KEY ([CondicionId]) REFERENCES [dbo].[ResearchCondicion] ([CondicionId]),
)
