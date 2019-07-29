CREATE TABLE [dbo].[Estadio] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR (50)  NULL,
    [MaterialId] INT           NOT NULL,
    CONSTRAINT [PK_Estadio] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Estadio_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

