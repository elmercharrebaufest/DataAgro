CREATE TABLE [dbo].[Estado] (
    [EstadoId]    INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Estado] PRIMARY KEY CLUSTERED ([EstadoId] ASC)
);

