CREATE TABLE [dbo].[ConfirmaAltaEstadoLote] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR(50) NOT NULL,
    [CodigoConfirmaAltaEstadoLote] INT NOT NULL,
    CONSTRAINT [PK_ConfirmaAltaEstadoLote] PRIMARY KEY CLUSTERED ([Id] ASC)
);