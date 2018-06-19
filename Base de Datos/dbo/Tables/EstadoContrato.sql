CREATE TABLE EstadoContrato (
    [EstadoContratoId]   INT IDENTITY(1,1),
    [Descripcion] VARCHAR (50) NOT NULL,
	[Orden] INT NOT NULL
    CONSTRAINT [PK_EstadoContrato] PRIMARY KEY CLUSTERED ([EstadoContratoId] ASC)
)

