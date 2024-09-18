CREATE TABLE [dbo].[ConfirmaAltaEstado] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR(50) NOT NULL,
    [CodigoConfirmaAltaEstado] INT NOT NULL,
    CONSTRAINT [PK_ConfirmaAltaEstado] PRIMARY KEY CLUSTERED ([Id] ASC)
);