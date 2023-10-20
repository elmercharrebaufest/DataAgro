CREATE TABLE [dbo].[Manuales]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Titulo] VARCHAR (100) NOT NULL,
	[Descripcion] VARCHAR(MAX) NOT NULL,
	[Path] VARCHAR(200) NOT NULL,
	[FechaUltimaActualizacion] DATETIME NOT NULL,
	[Version] INT NOT NULL,
	[CantidadVisitas] INT NOT NULL,
	CONSTRAINT [PK_Manuales] PRIMARY KEY CLUSTERED ([Id] ASC)
)