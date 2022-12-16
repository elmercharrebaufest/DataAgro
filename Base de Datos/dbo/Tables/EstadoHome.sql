CREATE TABLE [dbo].[EstadoHome] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NOT NULL,
    [Color] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_EstadoHome] PRIMARY KEY CLUSTERED ([Id] ASC)
);