CREATE TABLE [dbo].[TipoTelefono] (
    [TipoTelefonoId] INT          NOT NULL,
    [Descripcion]    VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TipoTelefono] PRIMARY KEY CLUSTERED ([TipoTelefonoId] ASC)
);

