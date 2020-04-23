CREATE TABLE [dbo].[ReporteCompraNetPricingCampania] (
	[Id]    INT           IDENTITY (1, 1) NOT NULL,    
	[Material]         varchar(50) NOT NULL,
	[MaterialId]         int NOT NULL,
	[Campania]         varchar(50) NOT NULL,
	[CampaniaId]         int NOT NULL,
	[Pricing] NUMERIC(18, 2) NOT NULL, 
	[SanLorenzo] NUMERIC(18, 2) NOT NULL, 
	[Acopio] NUMERIC(18, 2) NOT NULL, 
    CONSTRAINT [PK_ReporteCompraNetPricingCampania] PRIMARY KEY CLUSTERED ([Id] ASC)
);

