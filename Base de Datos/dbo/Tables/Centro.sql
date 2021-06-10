CREATE TABLE [dbo].[Centro] (
    [Id]	INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR(50) NOT NULL,
    [CodigoSap]           VARCHAR(20) NOT NULL
    CONSTRAINT [PK_Centro] PRIMARY KEY CLUSTERED ([Id] ASC), 
    [Acopio] BIT NOT NULL DEFAULT 0, 
    [ValidaRedespacho] BIT NOT NULL DEFAULT 0
);

