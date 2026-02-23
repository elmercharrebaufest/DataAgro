CREATE TABLE [dbo].[ControlDeBoletosSeguimiento]
(
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    ControlDeBoletosId INT NOT NULL, 
    BolsaCompraNetId INT NOT NULL, 
    BolsaSellado VARCHAR(100) NOT NULL, 
    FechaEnviadoFirma DATETIME NULL,
    FechaEnvio DATETIME NULL,
    FechaEnvioAfip DATETIME NULL,
    FechaEnvioBolsa DATETIME NULL,
    FechaRecepBoleto DATETIME NULL,
    FechaRecibFirma DATETIME NULL,
    FechaVueltaAfip DATETIME NULL,
    FechaVueltaBolsa DATETIME NULL,
    FechaAcopio DATETIME NULL, 
    BoletoCompraNetId INT NOT NULL, 
    ObsCtrlBoleto VARCHAR(500) NULL, 
    ObsCtrlBoleto2 VARCHAR(500) NULL, 
    FechaCreacion DATETIME NOT NULL DEFAULT SYSDATETIME(), 
    FechaModificacion DATETIME NULL,

    CONSTRAINT FK_Seguimiento_ControlDeBoletos 
        FOREIGN KEY (ControlDeBoletosId) 
        REFERENCES [dbo].[ControlDeBoletos](Id),

    CONSTRAINT FK_Seguimiento_Bolsa 
        FOREIGN KEY (BolsaCompraNetId) 
        REFERENCES [dbo].[BolsaCompraNet](Id),

    CONSTRAINT FK_Seguimiento_Boleta 
        FOREIGN KEY (BoletoCompraNetId) 
        REFERENCES [dbo].[BoletoCompraNet](Id)
);
