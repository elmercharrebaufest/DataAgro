CREATE TABLE [dbo].[TipoRangoConfirmacionAutomatica] (
    [Id]	INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR(150) NOT NULL,
    CONSTRAINT [PK_TipoRangoConfirmacionAutomatica] PRIMARY KEY CLUSTERED ([Id] ASC)
);

