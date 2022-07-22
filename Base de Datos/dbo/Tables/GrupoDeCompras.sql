CREATE TABLE [dbo].[GrupoDeCompras] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NULL,
    [Corredor] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_GrupoDeCompras] PRIMARY KEY CLUSTERED ([Id] ASC)
);

