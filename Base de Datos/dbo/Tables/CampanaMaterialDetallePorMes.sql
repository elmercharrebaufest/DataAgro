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
    [CampanaMaterialDetalleId]       INT        NULL,    
    [ComercialId]             INT        NULL,
    [CorredorId] INT NULL, 
    CONSTRAINT [PK_CampanaMaterialDetallePorMes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampanaMaterialDetallePorMes_Contacto] FOREIGN KEY ([CorredorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_CampanaMaterialDetallePorMes_CampanaMaterialDetalle] FOREIGN KEY ([CampanaMaterialDetalleId]) REFERENCES [dbo].[CampanaMaterialDetalle] ([Id]),
	CONSTRAINT [FK_CampanaMaterialDetallePorMes_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);

