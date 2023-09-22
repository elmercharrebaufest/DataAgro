CREATE TABLE [dbo].[LogAnulacionContrato]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [Fecha] DATETIME NOT NULL, 
    [NegocioId] INT NOT NULL,
    [TipoNegocio] VARCHAR(50) NULL,
    [ComercialId] INT NOT NULL,
    [ContratoSAP] NVARCHAR(15) NULL,
    [FijacionSAP] NVARCHAR(15) NULL,
    [CantidadKilos] FLOAT NULL,
    [KilosPendientes] FLOAT NULL,
    CONSTRAINT [FK_LogAnulacionContrato_Negocio] FOREIGN KEY (NegocioId) REFERENCES [Negocio]([Id]),
    CONSTRAINT [FK_LogAnulacionContrato_Comercial] FOREIGN KEY (ComercialId) REFERENCES [Comercial]([ComercialId]),
    CONSTRAINT [PK_LogAnulacionContrato] PRIMARY KEY CLUSTERED ([Id] ASC)
)