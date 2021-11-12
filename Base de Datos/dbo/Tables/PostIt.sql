CREATE TABLE [dbo].[PostIt] (
    [Id] INT  IDENTITY (1, 1) NOT NULL,
	[ComercialId] INT  NOT NULL,
    [Texto]       NVARCHAR (255) NULL,
    CONSTRAINT [PK_PostIt] PRIMARY KEY CLUSTERED ([Id] ASC)
);

