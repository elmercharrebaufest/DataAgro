CREATE TABLE [dbo].[ResearchHumedadSuelo]
(
	[HumedadSueloId] INT IDENTITY (1, 1) NOT NULL,
	[Descripcion] VARCHAR (50) NULL,
	CONSTRAINT [PK_ResearchHumedadSuelo] PRIMARY KEY CLUSTERED ([HumedadSueloId] ASC),
)
