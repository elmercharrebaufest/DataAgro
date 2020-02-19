CREATE TABLE [dbo].[TipoResearch] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]       VARCHAR(MAX)	NOT NULL,
    
    CONSTRAINT [PK_TipoResearch] PRIMARY KEY CLUSTERED ([Id] ASC),	
);

