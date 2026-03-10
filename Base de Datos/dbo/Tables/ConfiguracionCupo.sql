CREATE TABLE [dbo].[ConfiguracionCupo] (
	[Id]					INT IDENTITY (1, 1) NOT NULL,
	[MaterialId]			INT NOT NULL,
	[Fecha]					DATETIME NOT NULL, 
	[LimiteCupo]			INT NOT NULL, 
	[CentroId]				INT NOT NULL, 
	[CierreCupera]			BIT NOT NULL DEFAULT 0, 
	[LimiteAlgoritmo]		INT NOT NULL DEFAULT 0, 
	[LiberarCupera]			BIT NOT NULL DEFAULT 0, 
	[LimiteAnterior]		INT NOT NULL DEFAULT 0, 
	[LimiteDescarga]		INT NOT NULL DEFAULT 0, 
	CONSTRAINT [PK_ConfiguracionCupo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_ConfiguracionCupo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
	CONSTRAINT [FK_ConfiguracionCupo_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id])
);

GO
CREATE NONCLUSTERED INDEX NDX_MaterialId_Fecha 
ON [dbo].[ConfiguracionCupo] ([MaterialId],[Fecha])

GO
CREATE NONCLUSTERED INDEX NDX_CentroId_MaterialId_Fecha 
ON [dbo].[ConfiguracionCupo] ([CentroId],[MaterialId],[Fecha]) 
INCLUDE ([LimiteCupo],[CierreCupera],[LimiteAlgoritmo],[LiberarCupera],[LimiteAnterior],[LimiteDescarga])

GO