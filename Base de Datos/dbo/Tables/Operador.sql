CREATE TABLE [dbo].[Operador] (
    [Id]	INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR(50) NOT NULL
    
    CONSTRAINT [PK_Operador] PRIMARY KEY CLUSTERED ([Id] ASC)
);

