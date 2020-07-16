CREATE TABLE [dbo].[CampanaMaterialDetallePorMes] (
    [Id] INT        IDENTITY (1, 1) NOT NULL,
	[Contrato]       VARCHAR(MAX)        NULL, 
	[CorredorCuit]   VARCHAR(MAX) NULL,
    [Fecha]                     DATETIME        NULL,
    [PendienteAplicar]               FLOAT      NULL,
	[PendienteAFijar]               FLOAT      NULL,
	[ToneladaAmpliada]               FLOAT      NULL,
	[ToneladaAnulada]               FLOAT      NULL,
	[ToneladaAplicada]               FLOAT      NULL,
	[ToneladaContrato]               FLOAT      NULL,
	[ToneladaFijada]               FLOAT      NULL,
	[ClaseDoc]       VARCHAR(MAX)        NULL, 
	[Clasificacion]       VARCHAR(MAX)        NULL, 
    [CampanaMateriaDetalleId]       INT        NULL,    
    [ComercialId]             INT        NULL,
    CONSTRAINT [PK_CampanaMaterialDetallePorMes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampanaMaterialDetallePorMes_CampanaMaterialDetalle] FOREIGN KEY ([CampanaMateriaDetalleId]) REFERENCES [dbo].[CampanaMaterialDetalle] ([Id])
);

