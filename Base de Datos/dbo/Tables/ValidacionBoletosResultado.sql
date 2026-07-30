CREATE TABLE [dbo].[ValidacionBoletosResultado] (
    Id 						  INT IDENTITY(1,1) PRIMARY KEY,
    ValidacionBoletosId 	  INT NOT NULL,
    Resultado                 VARCHAR(200),
    FechaCreacion 			  DATETIME NOT NULL,
    FechaModificacion 		  DATETIME NULL,
	FechaRechazo			  DATETIME NULL,
    CONSTRAINT FK_ValidacionBoletosResultado_Validacion FOREIGN KEY (ValidacionBoletosId) REFERENCES [dbo].[ValidacionBoletos](Id)
);
