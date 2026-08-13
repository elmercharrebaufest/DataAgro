CREATE TABLE [dbo].[ConfiguracionesDistribucionPlanta] (
    [Id]                     INT            NOT NULL,
    [PlantaCodigo]           VARCHAR (10)   NOT NULL,
    [PlantaNombre]           NVARCHAR (200) NULL,
    [CupoKg]                 INT            NOT NULL CONSTRAINT [DF_ConfiguracionesDistribucionPlanta_CupoKg] DEFAULT ((30000)),
    [CuitMaxPct]             DECIMAL (5, 4) NOT NULL CONSTRAINT [DF_ConfiguracionesDistribucionPlanta_CuitMaxPct] DEFAULT ((0.3000)),
    [CosechasValidas]        NVARCHAR (MAX) NULL,
    [ClasesExcluidas]        NVARCHAR (MAX) NULL,
    [LimitesPredeterminados] NVARCHAR (MAX) NULL,
    [PreciosReferencia]      NVARCHAR (MAX) NULL,
    [CuotasPorOperador]      NVARCHAR (MAX) NULL,
    [CuotasPorClase]         NVARCHAR (MAX) NULL,
    [UltimaActualizacion]    DATETIME       NULL,
    CONSTRAINT [PK_ConfiguracionesDistribucionPlanta] PRIMARY KEY CLUSTERED ([Id] ASC)
);
