CREATE TABLE [dbo].[TipoServicio]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Descripcion] VARCHAR(50) NOT NULL,
	[CodigoSAP] VARCHAR(50) NULL,
);
