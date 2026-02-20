CREATE TABLE [dbo].[ControlDeBoletosSeguimiento]
(
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    ControlDeBoletosId INT NOT NULL, 
    BolsaCompraNetId INT NOT NULL, 
    BolsaSellado VARCHAR(100) NOT NULL, 
    FechaEnviadoFirma DATETIME2 NULL,
    FechaEnvio DATETIME2 NULL,
    FechaEnvioAfip DATETIME2 NULL,
    FechaEnvioBolsa DATETIME2 NULL,
    FechaRecepBoleto DATETIME2 NULL,
    FechaRecibFirma DATETIME2 NULL,
    FechaVueltaAfip DATETIME2 NULL,
    FechaVueltaBolsa DATETIME2 NULL,
    FechaAcopio DATETIME2 NULL, 
    BoletoCompraNetId INT NOT NULL, 
    ObsCtrlBoleto VARCHAR(500) NULL, 
    ObsCtrlBoleto2 VARCHAR(500) NULL, 
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
    FechaModificacion DATETIME2 NULL,

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
