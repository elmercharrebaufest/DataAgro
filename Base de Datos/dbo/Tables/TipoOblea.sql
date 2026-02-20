CREATE TABLE [dbo].[TipoOblea]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Codigo] VARCHAR(50) NOT NULL, 
    [Descripcion] VARCHAR(200) NOT NULL,
    CONSTRAINT [PK_TipoOblea] PRIMARY KEY CLUSTERED ([Id] ASC)
);