CREATE TABLE [dbo].[Reportes] (
    [Identificador] VARCHAR (32)    NOT NULL,
    [Contenido]     VARBINARY (MAX) NULL,
    [FileName]      VARCHAR (100)   NULL,
    CONSTRAINT [PK_Reportes] PRIMARY KEY CLUSTERED ([Identificador] ASC)
);

