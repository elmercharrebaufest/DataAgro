CREATE TABLE [dbo].[BoletoVenta](
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    
    CONSTRAINT [PK_dbo.BoletoVenta] PRIMARY KEY CLUSTERED ([Id] ASC)
);
