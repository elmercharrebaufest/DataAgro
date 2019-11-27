CREATE TABLE [dbo].[RolPermiso] (
    [Id]					INT IDENTITY (1, 1) NOT NULL,
    [RolId]			INT NOT NULL,
    [Permiso]			INT NOT NULL,
    CONSTRAINT [PK_RolPermiso] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RolPermiso_Rol] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Rol] ([Id]) ON DELETE CASCADE,
);

GO
CREATE NONCLUSTERED INDEX [IX_Rol_Id]
    ON [dbo].[RolPermiso]([RolId] ASC);
