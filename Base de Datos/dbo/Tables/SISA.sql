CREATE TABLE [dbo].[SISA] (
    [Id]							INT            IDENTITY (1, 1) NOT NULL,
    [CUIT]							NVARCHAR (100)	NOT NULL,
    [RazonSocial]					VARCHAR (200)	NOT NULL,
    [EstadoCuit]					INT				NOT NULL,
    [FechaVigenciaEstado]			DATETIME		NULL,
    [FechaNotifDFEEstado]			DATETIME		NULL,
    [CBU]							VARCHAR (500)	NULL,
    [FechaActCBU]					DATETIME		NULL,
	[CodCategoria]					INT				NULL,
    [Categoria]					    VARCHAR (500)	NOT NULL,
    [SituacionCategoria]			NVARCHAR (10)	NOT NULL,
    [FechaVigenciaCategoria]		DATETIME		NULL,
    [FechaNotifDFECategoria]		DATETIME		NULL,
    [Observaciones]					VARCHAR (1000)	NULL,
    [FechaGeneracion]				DATETIME		NULL,
    CONSTRAINT [PK_SISA]			PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [CUIT]
    ON [dbo].[SISA]([CUIT] ASC);