CREATE TABLE [dbo].[ProveedorRol] (
    [Proveedor_ProveedorId]       INT     NOT NULL,
    [Rol_Id] INT NOT NULL,
    CONSTRAINT [PK_ProveedorRol] PRIMARY KEY CLUSTERED ([Proveedor_ProveedorId] ASC,[Rol_Id] ASC) ,
    CONSTRAINT [FK_ProveedorRol_Proveedor] FOREIGN KEY ([Proveedor_ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProveedorRol_Rol] FOREIGN KEY ([Rol_Id]) REFERENCES [dbo].[Rol] ([Id]) ON DELETE CASCADE    
);

GO
CREATE NONCLUSTERED INDEX [IX_Usuario_Id]
    ON [dbo].[ProveedorRol]([Proveedor_ProveedorId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Rol_Id]
    ON [dbo].[ProveedorRol]([Rol_Id] ASC);
