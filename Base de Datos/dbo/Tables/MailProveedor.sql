CREATE TABLE [dbo].[MailProveedor] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Pesificado]     VARCHAR(100)           NOT NULL,
   	[ProveedorId]			INT				NULL, 
    [DireccionSap]     VARCHAR(200)            NULL,
    [LocalidadSap]     VARCHAR(100)           NULL,
    [ProvinciaSap]     VARCHAR(30)           NULL,
    [CodigoPostalSap]     VARCHAR(20)           NULL,
    CONSTRAINT [PK_MailProveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MailProveedor_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor]([ProveedorId]),
);

