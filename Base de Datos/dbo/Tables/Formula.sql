CREATE TABLE [dbo].[Formula] (
    [Id]		INT         IDENTITY (1, 1) NOT NULL,
    [Inicio]		INT  NOT NULL,
	[CantDias]	INT NOT NULL,
	[CriterioId]INT NOT NULL,
	[CentroId] INT NOT NULL, 
    [Fecha] DATETIME NOT NULL DEFAULT getdate(), 
    [Usada] BIT NULL DEFAULT 0, 
    CONSTRAINT [FK_Formula_CriterioId] FOREIGN KEY ([CriterioId]) REFERENCES [Criterio]([Id]), 
	    CONSTRAINT [FK_Formula_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id])
);
