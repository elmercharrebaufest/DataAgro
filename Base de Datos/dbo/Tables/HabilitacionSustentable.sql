CREATE TABLE [dbo].[HabilitacionSustentable] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [Precio]                DECIMAL(11, 2) NOT NULL, 
    [MonedaId]              CHAR(5) NOT NULL, 
    [DesdeVigencia]		    DATETIME NOT NULL,
    [HastaVigencia]		    DATETIME NOT NULL,
    [TipoNegocioId] INT NOT NULL DEFAULT 3, 
    [DesdeEntrega] DATETIME NOT NULL, 
    [HastaEntrega] DATETIME NOT NULL,    
    [UsuarioCreadorId] INT NOT NULL, 
    [FechaCreacion] DATETIME NOT NULL, 
    CONSTRAINT [PK_HabilitacionSustentable] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_HabilitacionSustentable_Moneda] FOREIGN KEY ([MonedaId]) REFERENCES [dbo].Moneda ([MonedaId]),
	CONSTRAINT [FK_HabilitacionSustentable_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [dbo].TipoNegocio ([TipoNegocioId]),
	CONSTRAINT [FK_HabilitacionSustentable_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreadorId]) REFERENCES [Comercial]([ComercialId])
);

