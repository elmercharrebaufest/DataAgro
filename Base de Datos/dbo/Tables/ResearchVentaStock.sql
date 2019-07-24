CREATE TABLE [dbo].[ResearchVentaStock] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [LocalidadId] INT           NOT NULL,
	[MaterialId] INT           NOT NULL,
	[ComercialId] INT           NOT NULL,
	[FechaHora] DATETIME NOT NULL,
    [VendidoAprecio] DECIMAL (18,2) NOT NULL,
	[Almacenado] DECIMAL (18,2) NOT NULL,
    [Observaciones] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_ResearchVentaStock] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResearchVentaStock_Localidad] FOREIGN KEY ([LocalidadId]) REFERENCES [dbo].[Localidad] ([LocalidadId]),
	CONSTRAINT [FK_ResearchVentaStock_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
	CONSTRAINT [FK_ResearchVentaStock_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId])
);

