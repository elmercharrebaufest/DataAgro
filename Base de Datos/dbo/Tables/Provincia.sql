CREATE TABLE [dbo].[Provincia] (
    [ProvinciaId] INT          IDENTITY (1, 1) NOT NULL,
    [Nombre]      VARCHAR (50) NOT NULL,
    [Orden]		  INT	NULL,
    [HabilitadoVenta] BIT NOT NULL DEFAULT 0,
    [Inscripto] BIT NOT NULL DEFAULT 0,
    [CodigoConfirma]      VARCHAR (50) NULL,
    CONSTRAINT [PK_Provincia] PRIMARY KEY CLUSTERED ([ProvinciaId] ASC)
);

