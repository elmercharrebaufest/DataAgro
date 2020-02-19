CREATE TABLE [dbo].[TipoObjetivo] (
    [Id] INT         IDENTITY (1, 1) NOT NULL,
    [Descripcion]    VARCHAR (100) NOT NULL
    CONSTRAINT [PK_TipoObjetivo] PRIMARY KEY CLUSTERED ([Id] ASC)
);
