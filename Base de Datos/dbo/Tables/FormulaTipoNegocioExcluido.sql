CREATE TABLE [dbo].[FormulaTipoNegocioExcluido]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[FormulaId] INT NOT NULL,
	[TipoNegocioId] INT NOT NULL,
	CONSTRAINT [FK_FormulaTipoNegocioExcluido_TipoNegocioId] FOREIGN KEY ([TipoNegocioId]) REFERENCES [TipoNegocio]([TipoNegocioId]),
);
