CREATE TABLE [dbo].[HabilitacionPizarra] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [Dia]                   DATETIME NOT NULL,
    [DesdeVigencia]		    DATETIME NOT NULL,
    [HastaVigencia]		    DATETIME NOT NULL,    
    [MaterialId]              INT NOT NULL, 
    [TipoNegocioId] INT NOT NULL DEFAULT 3, 
    [DesdeEntrega] DATETIME NULL, 
    [HastaEntrega] DATETIME NULL, 
    [UsuarioCreadorId] INT NULL, 
    [FechaCreacion] DATETIME NULL, 
    [Habilitado] BIT NULL, 
    CONSTRAINT [PK_HabilitacionPizarra] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HabilitacionPizarra_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].Material ([MaterialId]),
	CONSTRAINT [FK_HabilitacionPizarra_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [dbo].TipoNegocio ([TipoNegocioId]),
	CONSTRAINT [FK_HabilitacionPizarra_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreadorId]) REFERENCES [Comercial]([ComercialId])

);

