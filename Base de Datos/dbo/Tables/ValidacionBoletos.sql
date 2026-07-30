CREATE TABLE [dbo].[ValidacionBoletos] (
    Id 						  INT IDENTITY(1,1) PRIMARY KEY,
    ControlDeBoletosId 		  INT NOT NULL,
    ValidacionBoletosEstadoId INT NOT NULL,
    FechaCreacion 			  DATETIME NOT NULL,
    FechaModificacion 		  DATETIME NULL,
	FechaRechazo			  DATETIME NULL,
	MotivoRechazo			  VARCHAR(200)
    CONSTRAINT FK_ValidacionBoletos_Boleto FOREIGN KEY (ControlDeBoletosId) REFERENCES [dbo].[ControlDeBoletos](Id),
    CONSTRAINT FK_ValidacionBoletos_EstadoValidacion FOREIGN KEY (ValidacionBoletosEstadoId) REFERENCES [dbo].[ValidacionBoletosEstado](Id)
);
