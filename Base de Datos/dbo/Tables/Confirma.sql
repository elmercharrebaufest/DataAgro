CREATE TABLE [dbo].[Confirma]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[NegocioId] INT NOT NULL, 
    [IsWebService] BIT NOT NULL, 
    [FechaGeneracion] DATETIME NOT NULL , 
    [ComercialId] INT NOT NULL, 
    [Version] INT NULL,
    [Archivo] VARCHAR(100) NULL,
    [FechaAnulacion] DATETIME NULL,
    CONSTRAINT [FK_Confirma_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [Negocio]([Id]),
	CONSTRAINT [FK_Confirma_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
)