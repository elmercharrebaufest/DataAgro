CREATE TABLE [dbo].[Campo] (
    [CampoId]                   INT           IDENTITY (1, 1) NOT NULL,
    [NroItem]                   INT           NOT NULL,
    [ProveedorId]               INT           NOT NULL,
    [LocalidadId]               INT           NOT NULL,
    [Latitud]               VARCHAR (50)  NULL,
    [KMZnombre]                 VARCHAR (500) NULL,
    [KMZfile]                   NVARCHAR (4000)          NULL,
    [ArrendaPropia]             INT           NULL,
    [HabilitadoSojaSustentable] BIT           NULL,
    [Longitud] VARCHAR(50) NULL, 
    [Nombre] VARCHAR(150) NULL, 
    [ComercialId] INT NULL, 
    CONSTRAINT [PK_Campo] PRIMARY KEY CLUSTERED ([CampoId] ASC),
    CONSTRAINT [FK_Campo_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_Campo_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
	CONSTRAINT [FK_Campo_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId])

);

