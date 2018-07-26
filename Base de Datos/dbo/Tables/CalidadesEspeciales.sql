CREATE TABLE [dbo].[CalidadesEspeciales]
(
	[Id]				INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]       VARCHAR(50) NOT NULL,
	[CodigoSap]			VARCHAR(40) NOT NULL
    CONSTRAINT [PK_CalidadesEspeciales] PRIMARY KEY CLUSTERED ([Id] ASC)
)
