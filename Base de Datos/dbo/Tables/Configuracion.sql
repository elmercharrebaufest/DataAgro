CREATE TABLE [dbo].[Configuracion] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [CantidadDias]			INT NULL DEFAULT 120,
    [ClaveStop]				VARCHAR(MAX) NULL DEFAULT '429DAC12-3BDC-6CC4-1A97-AD759F936E1B',
    [ConexionABMStop]		BIT NULL DEFAULT 1,
    [ConexionConsultaStop]	BIT NULL DEFAULT 1, 
    [TerminalStopId]		INT NULL DEFAULT 12,
    [CuitDestinoStop]		VARCHAR(MAX) NULL DEFAULT '20005894582',
    [CodigoLocalidadStop] INT NULL DEFAULT 18794, 
    [ImporteSustentable] DECIMAL(11, 2) NULL DEFAULT 8, 
    [ContratoAperturaPrecioPorcentajeDeComisionMaximo] DECIMAL(11, 2) NULL DEFAULT 3, 
    [DiasDiferimiento] INT NULL DEFAULT 120, 
    [CantidadAcuerdo] INT NULL DEFAULT 30000, 
    [CantidadDiasDolarizadoLimiteMaximo] INT NULL DEFAULT 120, 
    [CantidadMaxima] INT NULL DEFAULT 15000000, 
    CONSTRAINT [PK_Configuracion] PRIMARY KEY CLUSTERED ([Id] ASC),
);

