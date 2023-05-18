CREATE TABLE [dbo].[PantallaEnUso] (
    [Id]							INT IDENTITY (1, 1) NOT NULL,
    [NombrePantalla]				VARCHAR (100) NOT NULL,
    [ComercialId]			        INT NULL, 
    [FechaHoraInicioUso]            DATETIME NOT NULL,
    [FechaHoraFinUso]               DATETIME NULL,
    [UsuarioId]				        VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_PantallaEnUso]   PRIMARY KEY CLUSTERED ([Id] ASC)
);