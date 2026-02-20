CREATE TABLE [dbo].[ControlDeBoletos] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NegocioId INT NOT NULL,
    ControlDeBoletosEstadoId INT NOT NULL,
    EstadoConfirmaId INT NULL,
    EsConfirma BIT NOT NULL DEFAULT 0,
    AltaIdLoteConfirma INT NULL,
    IdentificadorConfirma INT NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    FechaModificacion DATETIME2 NULL,
    ControlIniciado BIT NOT NULL DEFAULT 0,
    ControlFinalizado BIT NOT NULL DEFAULT 0,
    CertificacionCompletada BIT NOT NULL DEFAULT 0,
    RegistroDatosOblea BIT NOT NULL DEFAULT 0,
    FechaControlIniciado DATETIME2 NULL,
    FechaControlFinalizado DATETIME2 NULL,
    FechaCertificacionCompletada DATETIME2 NULL,
    FechaRegistroDatosOblea DATETIME2 NULL,

    CONSTRAINT FK_ControlDeBoletos_Negocio 
        FOREIGN KEY (NegocioId) REFERENCES [dbo].[Negocio](Id),

    CONSTRAINT FK_ControlDeBoletos_Estado 
        FOREIGN KEY (ControlDeBoletosEstadoId) 
        REFERENCES [dbo].[ControlDeBoletosEstado](Id)
);
