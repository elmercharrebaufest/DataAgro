CREATE TABLE [dbo].[Boleto]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[NegocioId] INT NOT NULL, 
    [Version] INT NOT NULL, 
    [FechaGeneracion] DATETIME NOT NULL , 
    [ComercialId] INT NOT NULL, 
    [FechaAnulacion] DATETIME NULL,
    CONSTRAINT [FK_Boleto_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [Negocio]([Id]),
	CONSTRAINT [FK_Boleto_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])

)
