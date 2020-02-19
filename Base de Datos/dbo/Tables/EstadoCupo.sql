CREATE TABLE EstadoCupo (
    [Id]   INT IDENTITY(1,1),
    [Descripcion] VARCHAR (50) NOT NULL,
	[Orden] INT NOT NULL
    CONSTRAINT [PK_EstadoCupo] PRIMARY KEY CLUSTERED ([Id] ASC)
)

