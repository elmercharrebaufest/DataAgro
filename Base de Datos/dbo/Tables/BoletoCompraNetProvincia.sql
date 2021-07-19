
CREATE TABLE [dbo].[BoletoCompraNetProvincia](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoletoCompraNetId] [int] NOT NULL,
	[ProvinciaId] [int] NOT NULL,
 CONSTRAINT [PK_BoletoCompraNetProvincia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BoletoCompraNetProvincia]  WITH CHECK ADD  CONSTRAINT [FK_BoletoCompraNetProvincia_BolsaCompraNet] FOREIGN KEY([BoletoCompraNetId])
REFERENCES [dbo].[BolsaCompraNet] ([Id])
GO

ALTER TABLE [dbo].[BoletoCompraNetProvincia] CHECK CONSTRAINT [FK_BoletoCompraNetProvincia_BolsaCompraNet]
GO

ALTER TABLE [dbo].[BoletoCompraNetProvincia]  WITH CHECK ADD  CONSTRAINT [FK_BoletoCompraNetProvincia_Provincia] FOREIGN KEY([ProvinciaId])
REFERENCES [dbo].[Provincia] ([ProvinciaId])
GO

ALTER TABLE [dbo].[BoletoCompraNetProvincia] CHECK CONSTRAINT [FK_BoletoCompraNetProvincia_Provincia]
GO

