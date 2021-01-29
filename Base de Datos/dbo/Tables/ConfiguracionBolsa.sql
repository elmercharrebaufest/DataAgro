CREATE TABLE [dbo].[ConfiguracionBolsa]
(
	[Id] INT  IDENTITY (1, 1) NOT NULL,
	[ProvinciaId] INT NOT NULL,   
	DestinoId INT  NOT NULL,	
	[BolsaId] INT NOT NULL,
	
    CONSTRAINT [PK_dbo.ConfiguracionBolsa] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConfiguracionBolsa_Provincia] FOREIGN KEY ([ProvinciaId]) REFERENCES [Provincia]([ProvinciaId]), 
	CONSTRAINT [FK_ConfiguracionBolsa_BolsaCompraNet] FOREIGN KEY ([BolsaId]) REFERENCES [BolsaCompraNet]([Id]),
    CONSTRAINT [FK_ConfiguracionBolsa_Destino] FOREIGN KEY ([DestinoId]) REFERENCES [Centro](Id),

)
GO