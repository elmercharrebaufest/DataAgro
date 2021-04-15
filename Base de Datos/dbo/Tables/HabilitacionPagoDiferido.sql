CREATE TABLE [dbo].[HabilitacionPagoDiferido] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [CantidadDia]                   INT NOT NULL,
    [DesdeVigencia]		    DATETIME NOT NULL,
    [HastaVigencia]		    DATETIME NOT NULL,    
    [UsuarioCreadorId] INT NULL, 
    [FechaCreacion] DATETIME NULL, 
    [Tasa] DECIMAL(11, 2) NOT NULL, 
    [Habilitado] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_HabilitacionPagoDiferido] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_HabilitacionPagoDiferido_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreadorId]) REFERENCES [Comercial]([ComercialId])

);

