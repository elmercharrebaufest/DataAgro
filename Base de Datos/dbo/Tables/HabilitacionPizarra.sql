CREATE TABLE [dbo].[HabilitacionPizarra] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,
    [Dia]                   DATETIME NOT NULL,
    [DesdeVigencia]		    DATETIME NOT NULL,
    [HastaVigencia]		    DATETIME NOT NULL,    
    CONSTRAINT [PK_HabilitacionPizarra] PRIMARY KEY CLUSTERED ([Id] ASC)
);

