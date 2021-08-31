CREATE TABLE [dbo].[Camara] (
    [Id] INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]      VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Camara] PRIMARY KEY CLUSTERED ([Id] ASC)
);

