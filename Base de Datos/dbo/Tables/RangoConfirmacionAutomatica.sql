CREATE TABLE [dbo].[RangoConfirmacionAutomatica] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [PrecioMaximo]      DECIMAL(18,2)	NOT NULL,
    [PrecioMinimo]      DECIMAL(18,2)	NOT NULL,
    [MonedaId]			CHAR(5)			NOT NULL,
	[MaterialId]		INT NOT NULL,
    [FechaDesde]		DATETIME NULL, 
    [FechaHasta]		DATETIME NULL,  
	[ZonaId]			INT NULL,
	[Cantidad]			INT NULL,
	[DesdeMes]			INT NULL,
	[DesdeAnio]			INT NULL,
	[HastaMes]			INT NULL,
	[HastaAnio]			INT NULL,	
    [TipoNegocioId]		INT NOT NULL DEFAULT 2, 

    CONSTRAINT [PK_RangoConfirmacionAutomatica] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Confirmacion_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [Moneda]([MonedaId]),
	CONSTRAINT [FK_Confirmacion_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
	CONSTRAINT [FK_Confirmacion_GrupoCompras] FOREIGN KEY ([ZonaId]) REFERENCES [GrupoDeCompras]([Id]),
	CONSTRAINT [FK_Confirmacion_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [TipoNegocio]([TipoNegocioId])

);

