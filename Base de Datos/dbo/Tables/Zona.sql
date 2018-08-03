CREATE TABLE [dbo].[Zona] (
    [ZonaId]      INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Zona] PRIMARY KEY CLUSTERED ([ZonaId] ASC)
);

