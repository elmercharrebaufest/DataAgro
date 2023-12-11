CREATE TABLE [dbo].[ResearchCondicion]
(
	[CondicionId] INT IDENTITY (1, 1) NOT NULL,
	[Descripcion] VARCHAR (50) NULL,
	CONSTRAINT [PK_ResearchCondicion] PRIMARY KEY CLUSTERED ([CondicionId] ASC),
)
