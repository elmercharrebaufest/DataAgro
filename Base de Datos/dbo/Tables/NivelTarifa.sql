CREATE TABLE [dbo].[NivelTarifa] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [CodigoSap]           VARCHAR(20) NOT NULL,
	[Descripcion] VARCHAR (50)  NULL,
    CONSTRAINT [PK_NivelTarifa] PRIMARY KEY CLUSTERED ([Id] ASC),
);

