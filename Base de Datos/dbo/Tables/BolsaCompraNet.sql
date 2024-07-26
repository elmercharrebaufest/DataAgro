CREATE TABLE [dbo].[BolsaCompraNet]
(
	[Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    [CodigoSap]		   NVARCHAR (40) default 1 NOT NULL,
    [CodigoConfirma] VARCHAR(50) NULL,
    CONSTRAINT [PK_dbo.BolsaCompraNet] PRIMARY KEY CLUSTERED ([Id] ASC)
)
