CREATE TABLE [dbo].[ComercialRol] (
    [Comercial_ComercialId]       INT     NOT NULL,
    [Rol_Id] INT NOT NULL,
    CONSTRAINT [PK_ComercialRol] PRIMARY KEY CLUSTERED ([Comercial_ComercialId] ASC,[Rol_Id] ASC) ,
    CONSTRAINT [FK_ComercialRol_Comercial] FOREIGN KEY ([Comercial_ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ComercialRol_Rol] FOREIGN KEY ([Rol_Id]) REFERENCES [dbo].[Rol] ([Id]) ON DELETE CASCADE    
);

GO
CREATE NONCLUSTERED INDEX [IX_Usuario_Id]
    ON [dbo].[ComercialRol]([Comercial_ComercialId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Rol_Id]
    ON [dbo].[ComercialRol]([Rol_Id] ASC);

