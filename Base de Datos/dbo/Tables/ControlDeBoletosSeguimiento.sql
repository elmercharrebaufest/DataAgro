CREATE TABLE [dbo].[ControlDeBoletosSeguimiento]
(
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    ControlDeBoletosId INT NOT NULL,
    BolsaCompraNetId INT NULL, 
    [BoletoSapId] INT NULL, 
    BoletoSapCaracter VARCHAR(20) NULL, 
    BolsaSellado VARCHAR(100) NULL, 
    [FechaRecepcionBoleto] DATETIME NULL,
    [FechaEnvioFisicoBolsa] DATETIME NULL,
    [FechaRecepcionBoletoOriginal] DATETIME NULL,
    [FechaEnvioFirma] DATETIME NULL,
    [FechaEnvioBolsa] DATETIME NULL,
    [FechaEnvioAfip] DATETIME NULL,
    [FechaRecepcionFirma] DATETIME NULL,
    [FechaRecepcionBolsa] DATETIME NULL,
    [FechaRecepcionAfip] DATETIME NULL,
    [FechaEnvioSellado] DATETIME NULL,
    [RechazadoAfip]     VARCHAR(10) NULL,
    ObsCtrlBoleto VARCHAR(500) NULL, 
    ObsCtrlBoleto2 VARCHAR(500) NULL, 
    FechaCreacion DATETIME NOT NULL DEFAULT SYSDATETIME(), 
    FechaModificacion DATETIME NULL,

    CONSTRAINT FK_Seguimiento_ControlDeBoletos 
        FOREIGN KEY (ControlDeBoletosId) 
        REFERENCES [dbo].[ControlDeBoletos](Id),

    CONSTRAINT FK_Seguimiento_Bolsa 
        FOREIGN KEY ([BolsaCompraNetId]) 
        REFERENCES [dbo].[BolsaCompraNet](Id),

    CONSTRAINT FK_Seguimiento_Boleto
        FOREIGN KEY (BoletoSapId) 
        REFERENCES [dbo].[BoletoSap](Id)
);
