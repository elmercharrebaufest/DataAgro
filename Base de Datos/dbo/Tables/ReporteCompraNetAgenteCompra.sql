CREATE TABLE [dbo].[ReporteCompraNetAgenteCompra] (
 [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [MaterialId]         int NOT NULL,
        [Material]         varchar(50) NOT NULL,
[TipoAgenteId]        int NOT NULL,
    [TipoAgente]         varchar(50) NOT NULL,
    [Posicion]         varchar(50) NOT NULL,
    [PrecioPonderado]         numeric(18,2) NOT NULL,
    [OperadorId]         int NOT NULL,
    [Operador]         varchar(50) NOT NULL,
    [Cantidad]         numeric(18,2) NOT NULL,
	CONSTRAINT [PK_ReporteCompraNetAgenteCompra] PRIMARY KEY CLUSTERED ([Id] ASC)
);
