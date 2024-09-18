CREATE TABLE [dbo].[ConfirmaAltaEstadoDocumento] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR(50) NOT NULL,
    [CodigoConfirmaAltaEstadoDocumento] INT NOT NULL,
    CONSTRAINT [PK_ConfirmaAltaEstadoDocumento] PRIMARY KEY CLUSTERED ([Id] ASC)
);