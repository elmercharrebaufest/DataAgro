CREATE TABLE [dbo].[ProveedorComercial] (
    [ProveedorComercialId] INT NOT NULL,
    [ProveedorId]          INT NOT NULL,
    [NroItem]              INT NOT NULL,
    [ComercialId]          INT NOT NULL,
    CONSTRAINT [PK_ProveedorComercial] PRIMARY KEY CLUSTERED ([ProveedorComercialId] ASC),
    CONSTRAINT [FK_ProveedorComercial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
    CONSTRAINT [FK_ProveedorComercial_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId])
);

