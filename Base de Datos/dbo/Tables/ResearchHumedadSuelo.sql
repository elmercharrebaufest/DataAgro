CREATE TABLE [dbo].[ResearchHumedadSuelo]
(
	[HumedadSueloId] INT IDENTITY (1, 1) NOT NULL,
	[Descripcion] VARCHAR (100) NULL,
	CONSTRAINT [PK_ResearchHumedadSuelo] PRIMARY KEY CLUSTERED ([HumedadSueloId] ASC),
)
