CREATE TABLE [dbo].[Campaña] (
    [CampañaId]   INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NOT NULL,
    [Hasta] DATETIME NULL,
    [CodigoSIO] VARCHAR (50) NULL,
    CONSTRAINT [PK_Campaña] PRIMARY KEY CLUSTERED ([CampañaId] ASC)
);

