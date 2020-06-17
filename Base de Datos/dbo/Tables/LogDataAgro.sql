CREATE TABLE [dbo].[LogDataAgro]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [Usuario] VARCHAR(100) NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [DatoModificado] NVARCHAR(MAX) NOT NULL, 
    [NegocioId] INT NULL, 
    [CupoId] INT NULL, 
    [ProveedorId] INT NULL,

    [Clase] VARCHAR(100) NOT NULL, 
    [AccionRealizada] VARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_LogDataAgro] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LogDataAgro_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [dbo].[Negocio] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LogDataAgro_Cupo] FOREIGN KEY ([CupoId]) REFERENCES [dbo].[Cupo] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LogDataAgro_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]) ON DELETE CASCADE,
)
