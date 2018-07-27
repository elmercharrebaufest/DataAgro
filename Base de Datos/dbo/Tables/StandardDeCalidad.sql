CREATE TABLE [dbo].[StandardDeCalidad] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]       VARCHAR(50) NOT NULL,
	[CodigoSap]			VARCHAR(40) default 1 NOT NULL
    CONSTRAINT [PK_StandardDeCalidad] PRIMARY KEY CLUSTERED ([Id] ASC)
);

