CREATE TABLE [dbo].[Calidad]
(
	[Id]					INT IDENTITY (1, 1) NOT NULL,
    [ContratoId]			INT NOT NULL,
	[StandardDeCalidadId]	INT NOT NULL,
	[CalidadEspecialId]		INT NULL,	
	[Valor]					INT NULL
		

    CONSTRAINT [PK_Calidad] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Calidad_Contrato] FOREIGN KEY ([ContratoId]) REFERENCES [dbo].[Contrato] ([ContratoId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Calidad_StandardDeCalidad] FOREIGN KEY ([StandardDeCalidadId]) REFERENCES [StandardDeCalidad]([Id]),
	CONSTRAINT [FK_Calidad_CalidadEspecial] FOREIGN KEY ([CalidadEspecialId]) REFERENCES [CalidadEspecial]([Id])
)
