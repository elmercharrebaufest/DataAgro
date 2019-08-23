CREATE TABLE [dbo].[ZonaCupo] (
    [Id]	INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR(50) NOT NULL,
    [CodigoSap]           VARCHAR(20) NOT NULL
    CONSTRAINT [PK_ZonaCupo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

