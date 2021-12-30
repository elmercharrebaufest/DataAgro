CREATE TABLE [dbo].[TipoNegocioRangoConfirmacionAutomatica] (
    [Id]	INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR(150) NOT NULL,
    CONSTRAINT [PK_TipoNegocioRangoConfirmacionAutomatica] PRIMARY KEY CLUSTERED ([Id] ASC)
);

