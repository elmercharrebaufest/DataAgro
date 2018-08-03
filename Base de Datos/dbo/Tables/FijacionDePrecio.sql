CREATE TABLE [dbo].[FijacionDePrecio] (
    [FijacionId]  INT             IDENTITY (1, 1) NOT NULL,
    [MaterialId]  INT             NULL,
    [Precio]      DECIMAL (18, 2) NULL,
    [Fecha]       DATETIME        NULL,
    [ProveedorId] INT             NULL,
    CONSTRAINT [PK_FijacionDePrecio] PRIMARY KEY CLUSTERED ([FijacionId] ASC)
);

