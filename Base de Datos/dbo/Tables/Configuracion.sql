CREATE TABLE [dbo].[Configuracion] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [CantidadDias] INT NULL,
    CONSTRAINT [PK_Configuracion] PRIMARY KEY CLUSTERED ([Id] ASC),
);

