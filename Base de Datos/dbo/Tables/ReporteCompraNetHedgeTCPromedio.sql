CREATE TABLE [dbo].[ReporteCompraNetHedgeTCPromedio] (
 [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [PromedioTC]         NUMERIC(18, 2) NOT NULL,
    [TotalTC]         NUMERIC(18, 2) NOT NULL
    CONSTRAINT [PK_ReporteCompraNetHedgeTCPromedio] PRIMARY KEY CLUSTERED ([Id] ASC)
);

