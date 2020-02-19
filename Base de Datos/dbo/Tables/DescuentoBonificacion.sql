CREATE TABLE [dbo].[DescuentoBonificacion]
(
	[Id]				INT				IDENTITY (1, 1) NOT NULL,	
	[FechaDesde]		DATETIME		NULL,
	[FechaHasta]		DATETIME		NULL,
	[Importe]			DECIMAL(11, 2)	NOT NULL,
	[MonedaId]			CHAR(5)			NULL, 
	[Porcentaje]		DECIMAL(11, 2)	NOT NULL,
	[TipoDBId]			INT				NOT NULL, 
	[TipoPeriodoDBId]	INT				NOT NULL,
	[ContratoId]		INT             NOT NULL,    
    CONSTRAINT [PK_dbo.DescuentoBonificacion] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.DescuentoBonificacion_Contrato] FOREIGN KEY ([ContratoId]) REFERENCES [dbo].[Contrato] ([ContratoId]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.DescuentoBonificacion_TipoDB] FOREIGN KEY ([TipoDBId]) REFERENCES [dbo].[TipoDB] ([Id]),
	CONSTRAINT [FK_dbo.DescuentoBonificacion_TipoPeriodoDB] FOREIGN KEY ([TipoPeriodoDBId]) REFERENCES [dbo].[TipoPeriodoDB] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Contrato_Id]
    ON [dbo].[DescuentoBonificacion]([ContratoId] ASC);