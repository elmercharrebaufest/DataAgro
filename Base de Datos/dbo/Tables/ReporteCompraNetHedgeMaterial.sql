CREATE TABLE [dbo].[ReporteCompraNetHedgeMaterial] (
 [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [Material]         varchar(50) NOT NULL,
    [Disponible]         NUMERIC(18, 2) NOT NULL,
    [Forward]         NUMERIC(18, 2) NOT NULL,
    [NewCrop]         NUMERIC(18, 2) NOT NULL,
    CONSTRAINT [PK_ReporteCompraNetHedgeMaterial] PRIMARY KEY CLUSTERED ([Id] ASC)
);

