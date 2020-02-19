CREATE TABLE [dbo].[Partido] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50)  NULL,
    [ProvinciaId] INT NULL,

    CONSTRAINT [PK_Partido] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Partido_Provincia] FOREIGN KEY ([ProvinciaId]) REFERENCES [dbo].[Provincia] ([ProvinciaId]),
    
);

