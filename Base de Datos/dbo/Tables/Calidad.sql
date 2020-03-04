CREATE TABLE [dbo].[Calidad]
(
	[Id]					INT IDENTITY (1, 1) NOT NULL,
	[StandardDeCalidadId]	INT NOT NULL,
	[CalidadEspecialId]		INT NULL,	
	[Valor]					DECIMAL(18,2) NULL,
	[PorcentajeDesde]			DECIMAL(18,2) NULL,
	[PorcentajeHasta]			DECIMAL(18,2) NULL,		

    [NegocioId] INT NOT NULL, 
    CONSTRAINT [PK_Calidad] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Calidad_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [dbo].[Negocio] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Calidad_StandardDeCalidad] FOREIGN KEY ([StandardDeCalidadId]) REFERENCES [StandardDeCalidad]([Id]),
	CONSTRAINT [FK_Calidad_CalidadEspecial] FOREIGN KEY ([CalidadEspecialId]) REFERENCES [CalidadEspecial]([Id])
)
