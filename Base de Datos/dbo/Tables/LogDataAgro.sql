CREATE TABLE [dbo].[LogDataAgro]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [Usuario] VARCHAR(100) NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [DatoModificado] NVARCHAR(MAX) NOT NULL, 
    [ClaseId] INT NOT NULL, 
    [Clase] VARCHAR(100) NOT NULL, 
    [AccionRealizada] VARCHAR(100) NOT NULL, 

    [Tipo] VARCHAR(100) NOT NULL, 
    [Descripcion] VARCHAR(250) NOT NULL DEFAULT '', 
    [ProveedorId] INT NULL, 
	[CorredorId] INT NULL, 

    CONSTRAINT [FK_LogDataAgro_Proveedor] FOREIGN KEY (ProveedorId) REFERENCES [Proveedor]([ProveedorId]),
    CONSTRAINT [FK_LogDataAgro_Corredor] FOREIGN KEY (CorredorId) REFERENCES [Proveedor]([ProveedorId]),
    CONSTRAINT [PK_LogDataAgro] PRIMARY KEY CLUSTERED ([Id] ASC)
)

go
CREATE NONCLUSTERED INDEX [ndx_ClaseID] ON [dbo].[LogDataAgro]
(
	[ClaseId] ASC,
	[Clase] ASC
)
INCLUDE ( 	[Id],
	[Usuario],
	[Fecha],
	[DatoModificado],
	[AccionRealizada],
	[Tipo],
	[Descripcion],
	[ProveedorId],
	[CorredorId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO