CREATE TABLE [dbo].[TipoDeCambio]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR(50) NOT NULL,
    CONSTRAINT [PK_TipoDeCambio] PRIMARY KEY CLUSTERED ([Id] ASC)
)
