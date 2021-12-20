CREATE TABLE [dbo].[Formula] (
    [Id]		INT         IDENTITY (1, 1) NOT NULL,
    [CuposDesde]		DATETIME  NOT NULL,
	[CuposHasta]	DATETIME NOT NULL,
	[CriterioId]INT NOT NULL,
	[CentroId] INT NOT NULL, 
    [Fecha] DATETIME NOT NULL DEFAULT getdate(), 
    [Usada] BIT NULL DEFAULT 0, 
	[NegociosDesde]		DATETIME  NOT NULL,
	[NegociosHasta]	DATETIME NOT NULL,
    [MaterialId] INT NOT NULL DEFAULT 1, 

    CONSTRAINT [FK_Formula_CriterioId] FOREIGN KEY ([CriterioId]) REFERENCES [Criterio]([Id]), 
	CONSTRAINT [FK_Formula_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id]),
	CONSTRAINT [FK_Formula_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),

);
