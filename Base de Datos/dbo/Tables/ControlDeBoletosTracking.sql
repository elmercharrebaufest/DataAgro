CREATE TABLE ControlDeBoletoTracking (
    Id INT IDENTITY(1,1) NOT NULL,
    ControlDeBoletosId INT NOT NULL,
    Accion VARCHAR(200),
    Resultado VARCHAR(200),
    ValorAnterior VARCHAR(200),
    ValorNuevo VARCHAR(200),
    FechaModificacion DATETIME NOT NULL,
    UsuarioModificacion VARCHAR(200),
    CONSTRAINT PK_ControlDeBoletoTracking PRIMARY KEY (Id),
    CONSTRAINT FK_ControlDeBoletoTracking_ControlDeBoletos FOREIGN KEY (ControlDeBoletosId) REFERENCES ControlDeBoletos (Id)
);