CREATE TABLE [dbo].[ClausulaCartaOferta]
(
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [Discriminator]     VARCHAR(250) NOT NULL,
    [Orden]             INT NOT NULL, 
    [Estado]            BIT NOT NULL, 
    [Clausula]          VARCHAR(MAX) NOT NULL,
	CONSTRAINT [PK_ClausulaCartaOferta] PRIMARY KEY CLUSTERED ([Id] ASC),
)
