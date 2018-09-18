CREATE TABLE [dbo].[Provincia] (
    [ProvinciaId] INT          IDENTITY (1, 1) NOT NULL,
    [Nombre]      VARCHAR (50) NOT NULL,
	[Orden]		  INT	NULL,	
    CONSTRAINT [PK_Provincia] PRIMARY KEY CLUSTERED ([ProvinciaId] ASC)
);

