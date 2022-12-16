CREATE TABLE [dbo].[CapacidadProductiva] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,
    [ProveedorId]        INT        NOT NULL,
    [MaterialId]         INT        NOT NULL,
    [CampaniaId]          INT        NOT NULL,
    [Cantidad]          DECIMAL(18, 2)      NOT NULL,
    [UnidadMedida]      VARCHAR(50)        NOT NULL,
    [Porcentaje]          DECIMAL(18, 2)        NOT NULL,
    [FechaActualizacion] DATETIME NULL, 
    CONSTRAINT [PK_CapacidadProductiva] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CapacidadProductiva_Campania] FOREIGN KEY ([CampaniaId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
    CONSTRAINT [FK_CapacidadProductiva_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_CapacidadProductiva_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

