CREATE TABLE [dbo].[Perfil] (
    [PerfilId]    INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Perfil] PRIMARY KEY CLUSTERED ([PerfilId] ASC)
);

