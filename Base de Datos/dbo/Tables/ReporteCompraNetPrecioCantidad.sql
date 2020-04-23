CREATE TABLE [dbo].[ReporteCompraNetPrecioCantidad] (
 [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [Moneda]         VARCHAR(50) NOT NULL,
    [Cantidad]         NUMERIC(18, 2) NOT NULL,
	    CONSTRAINT [PK_ReporteCompraNetPrecioCantidad] PRIMARY KEY CLUSTERED ([Id] ASC)
);

