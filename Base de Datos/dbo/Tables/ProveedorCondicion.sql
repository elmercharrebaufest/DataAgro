CREATE TABLE [dbo].[ProveedorCondicion] (
    [ContactoCondicionId] INT NOT NULL,
    [ProveedorId]         INT NOT NULL,
    [NroItem]             INT NOT NULL,
    [CondicionId]         INT NOT NULL,
    CONSTRAINT [PK_ContactoCondicion] PRIMARY KEY CLUSTERED ([ContactoCondicionId] ASC),
    CONSTRAINT [FK_ContactoCondicion_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId])
);

