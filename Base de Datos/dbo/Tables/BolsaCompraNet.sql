CREATE TABLE [dbo].[BolsaCompraNet]
(
	[Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    
    CONSTRAINT [PK_dbo.BolsaCompraNet] PRIMARY KEY CLUSTERED ([Id] ASC)
)
