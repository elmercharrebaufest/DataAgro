CREATE TABLE [dbo].[CampanaMaterialDetalle] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,
    [CampanaId]          INT        NOT NULL,
    [ProveedorId]        INT        NOT NULL,
    [MaterialId]         INT        NOT NULL,
    CONSTRAINT [PK_CampanaMaterialDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampanaMaterialDetalle_Campana] FOREIGN KEY ([CampanaId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
    CONSTRAINT [FK_CampanaMaterialDetalle_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_CampanaMaterialDetalle_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

