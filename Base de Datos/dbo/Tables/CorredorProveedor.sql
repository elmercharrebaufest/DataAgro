CREATE TABLE [dbo].[CorredorProveedor] (
    [Id]					INT IDENTITY (1, 1) NOT NULL,
    [CorredorId]			INT NOT NULL,
    [ProveedorId]			INT NOT NULL,
    CONSTRAINT [PK_CorredorProveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CorredorProveedor_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_CorredorProveedor_Corredor] FOREIGN KEY ([CorredorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId])
);