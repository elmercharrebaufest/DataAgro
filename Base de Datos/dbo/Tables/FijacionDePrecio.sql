CREATE TABLE [dbo].[FijacionDePrecio] (
    [FijacionId]  INT             NOT NULL,
    [MaterialId]  INT             NULL,
    [Precio]      DECIMAL (18, 2) NULL,
    [Fecha]       DATETIME        NULL,
    [ProveedorId] INT             NULL,
    CONSTRAINT [PK_FijacionDePrecio] PRIMARY KEY CLUSTERED ([FijacionId] ASC)
);

