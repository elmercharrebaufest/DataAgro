CREATE TABLE [dbo].[ConfiguracionEspacioDinamico] (
    [Id]					INT IDENTITY (1, 1) NOT NULL,
    [MaterialId]			INT					NOT NULL,
	[Fecha]					DATETIME			NOT NULL, 
	[CantidadDeCupo]			INT					NOT NULL, 
	[CentroId]				INT					NOT NULL, 
	[ProveedorId]			INT					NOT NULL, 
    [ComercialId] INT NOT NULL, 
    [Calidad] VARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_ConfiguracionEspacioDinamico] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_ConfiguracionEspacioDinamico_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),
	CONSTRAINT [FK_ConfiguracionEspacioDinamico_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id]),
	CONSTRAINT [FK_ConfiguracionEspacioDinamico_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor]([ProveedorId]),
	CONSTRAINT [FK_ConfiguracionEspacioDinamico_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);