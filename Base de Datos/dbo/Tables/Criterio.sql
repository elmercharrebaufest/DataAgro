CREATE TABLE [dbo].[Criterio] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Discriminator]          VARCHAR(250)           NOT NULL,
    [Prioridad] INT NOT NULL, 
    [PadreId] INT NULL,

	 CONSTRAINT [PK_Criterio] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Criterio_Padre] FOREIGN KEY ([PadreId]) REFERENCES [Criterio]([Id]), 
);

