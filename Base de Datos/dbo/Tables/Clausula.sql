CREATE TABLE [dbo].[Clausula] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Discriminator]          VARCHAR(250)           NOT NULL,
    [Orden] INT NOT NULL, 
    [Estado] BIT NOT NULL, 
	CONSTRAINT [PK_Clausula] PRIMARY KEY CLUSTERED ([Id] ASC),
);

