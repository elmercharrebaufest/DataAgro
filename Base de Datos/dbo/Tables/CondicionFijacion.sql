CREATE TABLE [dbo].[CondicionFijacion] (
    [Id]	INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR(50) NOT NULL,
    [CodigoSap]           NVARCHAR(20) NOT NULL

    CONSTRAINT [PK_CondicionFijacion] PRIMARY KEY CLUSTERED ([Id] ASC), 
    [Habilitado] BIT NOT NULL DEFAULT 1
);

