CREATE TABLE [dbo].[ResearchCoeficienteCultivo]
(
	[CoeficienteCultivoId] INT IDENTITY (1, 1) NOT NULL,
	[MaterialId] INT NOT NULL,
	[Coeficiente] DECIMAL(4, 2) NOT NULL,
	CONSTRAINT [PK_ResearchCoeficienteCultivo] PRIMARY KEY CLUSTERED ([CoeficienteCultivoId] ASC),
	CONSTRAINT [FK_ResearchCoeficienteCultivo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
)
