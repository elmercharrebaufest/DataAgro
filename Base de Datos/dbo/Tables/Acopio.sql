CREATE TABLE [dbo].[Acopio] (
    [AcopioId]    INT           IDENTITY (1, 1) NOT NULL,
    [NroItem]     INT           NOT NULL,
    [ProveedorId] INT           NOT NULL,
    [LocalidadId] INT           NOT NULL,
    [Latitud] VARCHAR (50)  NULL,
    [KMZnombre]   VARCHAR (500) NULL,
    [KMZfile]     NVARCHAR (4000)          NULL,
    [Longitud] VARCHAR(50) NULL, 
    [Nombre] VARCHAR(150) NULL, 
    [ComercialId] INT NULL, 
    CONSTRAINT [PK_Acopio] PRIMARY KEY CLUSTERED ([AcopioId] ASC),
    CONSTRAINT [FK_Acopio_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_Acopio_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
    CONSTRAINT [FK_Acopio_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId])

);

