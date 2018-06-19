CREATE TABLE [dbo].[RG2300] (
    [Id]                 INT            NOT NULL,
    [CUIT]               VARCHAR (100)  NOT NULL,
    [RazonSocial]        VARCHAR (1000) NOT NULL,
    [Categoria]          VARCHAR (500)  NOT NULL,
    [Situacion]          VARCHAR (500)  NOT NULL,
    [CBU]                VARCHAR (500)  NULL,
    [FechaActCBU]        DATETIME       NULL,
    [FechaPubInclusion]  DATETIME       NULL,
    [FechaPubSuspension] DATETIME       NULL,
    [FechaLevSuspension] DATETIME       NULL,
    [FechaNotExclusion]  DATETIME       NULL,
    [FechaActRegistro]   DATETIME       NULL,
    [Observaciones]      VARCHAR (1000) NULL,
    [FechaGeneracion]    DATETIME       NULL,
    CONSTRAINT [PK_RG2300] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [CUIT]
    ON [dbo].[RG2300]([CUIT] ASC);

