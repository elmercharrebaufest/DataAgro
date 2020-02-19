CREATE TABLE [dbo].[HabilitacionFijacion] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [Dia]                   DATETIME NOT NULL,
    [MaterialId]            INT NOT NULL,
    CONSTRAINT [PK_HabilitacionFijacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HabilitacionFijacion_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].Material ([MaterialId])
);

