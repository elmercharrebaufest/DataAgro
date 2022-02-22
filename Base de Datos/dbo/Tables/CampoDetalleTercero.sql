CREATE TABLE [dbo].[CampoDetalleTercero] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [IdMoa]                   INT    NOT NULL,
    [ProveedorId]               INT           NOT NULL,
    [LocalidadId]               INT           NOT NULL,
    [Latitud]               VARCHAR (50)  NULL,
    [Estado]               VARCHAR (150)  NULL,
    [KMZnombre]                 VARCHAR (1500) NULL,
    [KMZfile]                   VARCHAR(MAX)          NULL,
    [Longitud] VARCHAR(50) NULL, 
    [Nombre] VARCHAR(150) NULL, 
    [Rinde] DECIMAL(11, 2) NULL, 
    [HectareasTotales] DECIMAL(11, 2) NULL, 
    [HectareasCultivables] DECIMAL(11, 2) NULL, 
    [MaterialId] INT NOT NULL, 
    [CampañaId] INT NOT NULL, 
    CONSTRAINT [PK_CampoDetalleTercero] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampoDetalleTercero_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_CampoDetalleTercero_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
    CONSTRAINT [FK_CampoDetalleTercero_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_CampoDetalleTercero_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].[Campaña] ([CampañaId])


);

