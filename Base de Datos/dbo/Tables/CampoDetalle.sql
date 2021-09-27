CREATE TABLE [dbo].[CampoDetalle] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [NroItem]                   INT           NOT NULL,
    [ProveedorId]               INT           NOT NULL,
    [LocalidadId]               INT           NOT NULL,
    [Latitud]               VARCHAR (50)  NULL,
    [KMZnombre]                 VARCHAR (1500) NULL,
    [KMZfile]                   VARCHAR(MAX)          NULL,
    [Longitud] VARCHAR(50) NULL, 
    [Nombre] VARCHAR(150) NULL, 
    [ComercialId] INT NULL, 
    [Rinde] DECIMAL(11, 2) NULL, 
    [HectareasTotales] DECIMAL(11, 2) NULL, 
    [HectareasCultivables] DECIMAL(11, 2) NULL, 
    [MaterialId] INT NOT NULL, 
    [ImportId] INT NULL, 
    [CampañaId] INT NULL, 
    CONSTRAINT [PK_CampoDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampoDetalle_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_CampoDetalle_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
    CONSTRAINT [FK_CampoDetalle_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_CampoDetalle_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
	CONSTRAINT [FK_CampoDetalle_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].[Campaña] ([CampañaId])


);

