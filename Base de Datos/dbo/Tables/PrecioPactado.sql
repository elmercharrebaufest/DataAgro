CREATE TABLE [dbo].[PrecioPactado]
(
	[Id]						INT				IDENTITY (1, 1) NOT NULL,	
	[FechaDesde]				DATETIME		NULL,
	[FechaHasta]				DATETIME		NULL,
	[Precio]					DECIMAL(11, 2)	NOT NULL,
	[MonedaPactadoId]			CHAR(5)			NOT NULL, 
	[ImportePactado]			DECIMAL(11, 2)	NULL,
	[MonedaImportePactadoId]	CHAR(5)			NULL, 
	[Porcentaje]				DECIMAL(11, 2)				NULL,
	[ContratoId]				INT             NOT NULL,    
    CONSTRAINT [PK_dbo.PrecioPactado] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.PrecioPactado_Contrato] FOREIGN KEY ([ContratoId]) REFERENCES [dbo].[Negocio] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.PrecioPactado_MonedaPactado] FOREIGN KEY ([MonedaPactadoId]) REFERENCES [dbo].[Moneda] ([MonedaId]),
	CONSTRAINT [FK_dbo.PrecioPactado_MonedaImportePactadoId] FOREIGN KEY ([MonedaImportePactadoId]) REFERENCES [dbo].[Moneda] ([MonedaId]),
);

GO
CREATE NONCLUSTERED INDEX [IX_Contrato_Id]
    ON [dbo].[PrecioPactado]([ContratoId] ASC);