CREATE TABLE [dbo].[Rol] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [Descripcion] VARCHAR (50)  NULL,   
    CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED ([Id] ASC)
);

