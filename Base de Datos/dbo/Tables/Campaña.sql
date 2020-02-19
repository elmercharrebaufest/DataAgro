CREATE TABLE [dbo].[Campaña] (
    [CampañaId]   INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Campaña] PRIMARY KEY CLUSTERED ([CampañaId] ASC)
);

