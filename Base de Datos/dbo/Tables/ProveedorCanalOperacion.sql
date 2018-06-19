CREATE TABLE [dbo].[ProveedorCanalOperacion] (
    [ContactoCanalOperacionId] INT        NOT NULL,
    [ProveedorId]              INT        NOT NULL,
    [NroItem]                  NCHAR (10) NOT NULL,
    [CanalOperacionId]         INT        NOT NULL,
    CONSTRAINT [PK_ContactoCanalOperacion] PRIMARY KEY CLUSTERED ([ContactoCanalOperacionId] ASC),
    CONSTRAINT [FK_ContactoCanalOperacion_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId])
);

