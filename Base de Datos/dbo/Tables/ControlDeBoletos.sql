CREATE TABLE [dbo].[ControlDeBoletos] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NegocioId INT NOT NULL,
    ControlDeBoletosEstadoId INT NOT NULL,
    CodigoEstadoConfirma INT NULL,
    EsConfirma BIT NOT NULL,
    IdentificadorConfirma INT NULL,
    FechaCreacion DATETIME NOT NULL,
    UsuarioCreacion VARCHAR(100) NOT NULL,
    FechaModificacion DATETIME NULL,
    UsuarioModificacion VARCHAR(100) NULL,
    
    -- Campos BIT para control de procesos
    ControlIniciado BIT NOT NULL DEFAULT 0,
    ControlFinalizado BIT NOT NULL DEFAULT 0,
    CertificacionCompletada BIT NOT NULL DEFAULT 0,
    RegistroDatosOblea BIT NOT NULL DEFAULT 0,
    FechaControlIniciado DATETIME NULL,
    FechaControlFinalizado DATETIME NULL,
    FechaCertificacionCompletada DATETIME NULL,
    FechaRegistroDatosOblea DATETIME NULL,
    CONSTRAINT FK_ControlDeBoletos_Negocio FOREIGN KEY (NegocioId) REFERENCES [dbo].[Negocio](Id),
    CONSTRAINT FK_ControlDeBoletos_Estado FOREIGN KEY (ControlDeBoletosEstadoId) REFERENCES [dbo].[ControlDeBoletosEstado](Id)
);