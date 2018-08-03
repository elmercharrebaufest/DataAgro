CREATE TABLE [dbo].[Interes] (
    [InteresId]   INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Interes] PRIMARY KEY CLUSTERED ([InteresId] ASC)
);

