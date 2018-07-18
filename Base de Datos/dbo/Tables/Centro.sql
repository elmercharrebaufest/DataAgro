CREATE TABLE [dbo].[Centro] (
    [Id]	INT NOT NULL,
    [Descripcion]         VARCHAR(50) NOT NULL,
    [CodigoSap]         INT NOT NULL
    CONSTRAINT [PK_Centro] PRIMARY KEY CLUSTERED ([Id] ASC)
);

