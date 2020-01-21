CREATE TABLE [dbo].[HabilitacionPizarra] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [Dia]                   DATETIME NOT NULL,
    [DesdeVigencia]		    DATETIME NOT NULL,
    [HastaVigencia]		    DATETIME NOT NULL,    
    [MaterialId]              INT NOT NULL, 
    CONSTRAINT [PK_HabilitacionPizarra] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HabilitacionPizarra_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].Material ([MaterialId])
);

