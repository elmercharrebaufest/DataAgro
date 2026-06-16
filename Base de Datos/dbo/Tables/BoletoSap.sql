CREATE TABLE [dbo].[BoletoSap]
(
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]       VARCHAR (100) NOT NULL,
    [Caracter]          VARCHAR (100) NOT NULL,
    [Confirma]          BIT,
    [CartaOferta]       BIT,
    [Fisico]            BIT,
    [Ninguno]           BIT,
    [SinBoleto]         BIT,
    CONSTRAINT [PK_dbo.BoletoSap] PRIMARY KEY CLUSTERED ([Id] ASC)
)
