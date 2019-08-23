CREATE TABLE [dbo].Pizarra (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Codigo]    VARCHAR (50)  NULL,
    [Descripcion]   VARCHAR (150) NULL,
    CONSTRAINT [PK_Pizarra] PRIMARY KEY CLUSTERED ([Id] ASC)
);

