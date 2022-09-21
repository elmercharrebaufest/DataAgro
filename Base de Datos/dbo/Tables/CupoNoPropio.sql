CREATE TABLE [dbo].[CupoNoPropio]
(
	[Id] INT NOT NULL IDENTITY (1, 1), 
    [CentroId] INT NOT NULL, 
    [Codigo] VARCHAR(50) NOT NULL, 
    [FechaIngreso] DATETIME NOT NULL, 
    [MaterialId] INT NOT NULL, 
    [CupoId] INT NULL, 
    [FechaAlta] DATETIME NOT NULL, 
    [Estado] INT NOT NULL, 
    [Disponible] BIT NOT NULL,
    CONSTRAINT [PK_CupoNoPropio] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_CupoNoPropio_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id]), 
    CONSTRAINT [FK_CupoNoPropio_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]), 
    CONSTRAINT [FK_CupoNoPropio_Cupo] FOREIGN KEY ([CupoId]) REFERENCES [Cupo]([Id]), 
    
)
