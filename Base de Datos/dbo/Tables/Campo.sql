CREATE TABLE [dbo].[Campo] (
    [CampoId]                   INT           IDENTITY (1, 1) NOT NULL,
    [NroItem]                   INT           NOT NULL,
    [ProveedorId]               INT           NOT NULL,
    [LocalidadId]               INT           NOT NULL,
    [ArrendaPropia]             INT           NULL,
    [HabilitadoSojaSustentable] BIT           NULL,
    CONSTRAINT [PK_Campo] PRIMARY KEY CLUSTERED ([CampoId] ASC),
    CONSTRAINT [FK_Campo_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_Campo_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId])
);

