CREATE TABLE [dbo].[Acopio] (
    [AcopioId]    INT           NOT NULL,
    [NroItem]     INT           NOT NULL,
    [ProveedorId] INT           NOT NULL,
    [LocalidadId] INT           NOT NULL,
    [Coordenadas] VARCHAR (50)  NULL,
    [KMZnombre]   VARCHAR (150) NULL,
    [KMZfile]     TEXT          NULL,
    CONSTRAINT [PK_Acopio] PRIMARY KEY CLUSTERED ([AcopioId] ASC),
    CONSTRAINT [FK_Acopio_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_Acopio_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId])
);

