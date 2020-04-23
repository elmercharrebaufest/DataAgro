CREATE TABLE [dbo].[ReporteCompraNetHedgeCargaObjetivo] (
 [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [PricingCumplido]         NUMERIC(18, 2) NOT NULL,
    [RemitirCumplido]         NUMERIC(18, 2) NOT NULL,
    [PricingObjetivo]         NUMERIC(18, 2) NOT NULL,
    [RemitirObjetivo]         NUMERIC(18, 2) NOT NULL,
    CONSTRAINT [PK_ReporteCompraNetHedgeCargaObjetivo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

