CREATE TABLE [dbo].[GrupoDeCompras] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NULL,
    CONSTRAINT [PK_GrupoDeCompras] PRIMARY KEY CLUSTERED ([Id] ASC)
);

