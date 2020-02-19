CREATE TABLE [dbo].[TipoHedgeMaterial] (
    [Id] INT         IDENTITY (1, 1) NOT NULL,
    [Descripcion]    VARCHAR (100) NOT NULL
    CONSTRAINT [PK_TipoHedgeMaterial] PRIMARY KEY CLUSTERED ([Id] ASC)
);
