CREATE TABLE [dbo].[ControlDeBoletosPreCertificacion]
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ControlDeBoletosId INT NOT NULL,
    Oblea VARCHAR(20) NULL,
    Rechazado VARCHAR(1) NULL,
    TipoObleaId INT NOT NULL,
    BolsaCompraNetId INT NULL,
    FechaCertificacion DATETIME NULL,
    FechaVencimiento DATETIME NULL,
    FechaCreacion DATETIME NOT NULL,
    FechaModificacion DATETIME NULL,
    CONSTRAINT FK_ControlDeBoletos_Certificacion FOREIGN KEY (ControlDeBoletosId) REFERENCES [dbo].[ControlDeBoletos](Id),
    CONSTRAINT FK_ControlDeBoletosPreCertificacion_Bolsa FOREIGN KEY (BolsaCompraNetId) REFERENCES [dbo].[BolsaCompraNet](Id),
    CONSTRAINT FK_ControlDeBoletosPreCertificacion_TipoOblea FOREIGN KEY (TipoObleaId) REFERENCES [dbo].[TipoOblea](Id)

);
