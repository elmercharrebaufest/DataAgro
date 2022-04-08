CREATE TABLE [dbo].[MailProveedor] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Pesificado]     VARCHAR(MAX)           NOT NULL,
   	[ProveedorId]			INT				NULL, 
    CONSTRAINT [PK_MailProveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MailProveedor_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor]([ProveedorId]),
);

