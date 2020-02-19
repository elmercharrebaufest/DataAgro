CREATE TABLE [dbo].[PostIt] (
    [ComercialId] INT  IDENTITY (1, 1) NOT NULL,
    [Texto]       NVARCHAR (255) NULL,
    CONSTRAINT [PK_PostIt] PRIMARY KEY CLUSTERED ([ComercialId] ASC)
);

