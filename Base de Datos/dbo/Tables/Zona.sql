CREATE TABLE [dbo].[Zona] (
    [Id]	INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR(50) NOT NULL,
    [CodigoSap]           VARCHAR(20) NOT NULL
    CONSTRAINT [PK_Zona] PRIMARY KEY CLUSTERED ([Id] ASC)
);