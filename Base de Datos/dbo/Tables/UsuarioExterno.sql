CREATE TABLE [dbo].[UsuarioExterno] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]            NVARCHAR(50)    NOT NULL,
    [ProveedorId]       INT             NOT NULL,
    [AceptaTyC]            BIT             NOT NULL, 
    CONSTRAINT [PK_UsuarioExterno] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsuarioExterno_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId])
);

