CREATE TABLE [dbo].[ControlDeBoletosPreCertificacion]
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ControlDeBoletosId INT NOT NULL,
    Oblea VARCHAR(18) NOT NULL,
    BolsaCompraNetId INT NOT NULL,
    FechaCertificacion DATETIME NOT NULL,
    FechaVencimiento DATETIME NOT NULL,
    FechaCreacion DATETIME NOT NULL,
    FechaModificacion DATETIME NULL,
    CONSTRAINT FK_ControlDeBoletos_Certificacion FOREIGN KEY (ControlDeBoletosId) REFERENCES [dbo].[ControlDeBoletos](Id),
    CONSTRAINT FK_ControlDeBoletos_Bolsa FOREIGN KEY (BolsaCompraNetId) REFERENCES [dbo].[BolsaCompraNet](Id)
);
