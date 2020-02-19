CREATE TABLE [dbo].[ProveedorDestinatario] (
    [ContactoDestinatarioId] INT IDENTITY (1, 1) NOT NULL,
    [ProveedorId]            INT NOT NULL,
    [NroItem]                INT NOT NULL,
    [DestinatarioId]         INT NOT NULL,
    CONSTRAINT [PK_ContactoDestinatario] PRIMARY KEY CLUSTERED ([ContactoDestinatarioId] ASC),
    CONSTRAINT [FK_ContactoDestinatario_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId])
);

