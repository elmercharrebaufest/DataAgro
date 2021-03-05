CREATE TABLE [dbo].[HabilitacionPagoDiferido] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [CantidadDia]                   INT NOT NULL,
    [DesdeVigencia]		    DATETIME NOT NULL,
    [HastaVigencia]		    DATETIME NOT NULL,    
    [MaterialId]              INT NOT NULL, 
    [TipoNegocioId] INT NOT NULL DEFAULT 3, 
    [UsuarioCreadorId] INT NULL, 
    [FechaCreacion] DATETIME NULL, 
    [Importe] DECIMAL(11, 2) NOT NULL, 
    CONSTRAINT [PK_HabilitacionPagoDiferido] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HabilitacionPagoDiferido_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].Material ([MaterialId]),
	CONSTRAINT [FK_HabilitacionPagoDiferido_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [dbo].TipoNegocio ([TipoNegocioId]),
	CONSTRAINT [FK_HabilitacionPagoDiferido_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreadorId]) REFERENCES [Comercial]([ComercialId])

);

