CREATE TABLE [dbo].[ManualesLogVisitas]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[ManualId] INT NOT NULL,
	[ComercialId] INT NOT NULL,
	[FechaVisita] DATETIME NOT NULL,
	CONSTRAINT [PK_ManualesLogVisitas] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_ManualesLogVisitas_Manuales] FOREIGN KEY ([ManualId]) REFERENCES [Manuales]([Id]),
	CONSTRAINT [FK_ManualesLogVisitas_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
)
