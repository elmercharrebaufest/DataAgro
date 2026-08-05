CREATE TABLE [dbo].[ValidacionBoletos]
(
    [Id]                          INT IDENTITY(1,1) PRIMARY KEY,
    [ControlDeBoletosId]          INT NOT NULL,
    [ValidacionBoletosEstadoId]   INT NOT NULL,

    [FechaCreacion]               DATETIME NOT NULL,
    [FechaModificacion]           DATETIME NULL,
    [FechaRechazo]                DATETIME NULL,

    [MotivoRechazo]               VARCHAR(200) NULL,
    [RequestId]                   VARCHAR(200) NULL,
    [UserId]                      VARCHAR(50) NULL,
    [RespuestaAgente]             NVARCHAR(MAX) NULL,
    [EstadoValidacionAgente]      VARCHAR(200) NULL,
    [AccionesRecomendadas]        VARCHAR(500) NULL,
    [Observacion] VARCHAR(500) NULL, 
    CONSTRAINT FK_ValidacionBoletos_Boleto
        FOREIGN KEY (ControlDeBoletosId)
        REFERENCES [dbo].[ControlDeBoletos](Id),

    CONSTRAINT FK_ValidacionBoletos_EstadoValidacion
        FOREIGN KEY (ValidacionBoletosEstadoId)
        REFERENCES [dbo].[ValidacionBoletosEstado](Id)
);
