CREATE TABLE ControlDeBoletoTracking (
    Id INT IDENTITY(1,1) NOT NULL,
    ControlDeBoletosId INT NOT NULL,
    EstadoDocumentoId INT,
    CUIT        INT,
    RazonSocial VARCHAR(200),
    Acciones NVARCHAR(MAX),
    FechaCreacion DATETIME NOT NULL,
    CONSTRAINT PK_ControlDeBoletoTracking PRIMARY KEY (Id),
    CONSTRAINT FK_ControlDeBoletoTracking_ControlDeBoletos FOREIGN KEY (ControlDeBoletosId) REFERENCES ControlDeBoletos (Id)
);
