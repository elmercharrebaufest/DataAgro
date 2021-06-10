CREATE TABLE [dbo].[EstadoPrecioMOA] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [MaterialId]      INT NOT NULL,   
    [Habilitado] BIT NULL, 
    CONSTRAINT [PK_EstadoPrecioMOA] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EstadoPrecioMOA_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

