CREATE TABLE [dbo].[ValidacionBoletosResultado] (
    [Id] 						  INT IDENTITY(1,1) PRIMARY KEY,
    [ValidacionBoletosId] 	      INT NOT NULL,
    [Campo]                       VARCHAR(200),
    [ValorDocumento]              VARCHAR(200),
    [ValorSistema]                VARCHAR(200),
    [Resultado]                   VARCHAR(200),
    [Severidad]                   VARCHAR(200),
    [Mensaje]                     VARCHAR(200),
    [TipoCoincidencia]            VARCHAR(200),
    [FechaCreacion] 			  DATETIME NOT NULL,
    CONSTRAINT FK_ValidacionBoletosResultado_Validacion FOREIGN KEY (ValidacionBoletosId) REFERENCES [dbo].[ValidacionBoletos](Id)
);
