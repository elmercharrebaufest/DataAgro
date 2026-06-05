CREATE TABLE [dbo].[BoletoSap]
(
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]       VARCHAR (100) NOT NULL,
    [Caracter]          VARCHAR (100) NOT NULL,  
    [BoletoCompraNetId] INT NULL,
    CONSTRAINT [PK_dbo.BoletoSap] PRIMARY KEY CLUSTERED ([Id] ASC)
)
