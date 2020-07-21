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
    CONSTRAINT [PK_LogDataAgro] PRIMARY KEY CLUSTERED ([Id] ASC)
)
