CREATE TABLE [dbo].[TipoAgenteCompra]
(
	[Id]					INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]				NVARCHAR(20) NOT NULL,	

    CONSTRAINT [PK_TipoAgenteCompra] PRIMARY KEY CLUSTERED ([Id] ASC)
)
