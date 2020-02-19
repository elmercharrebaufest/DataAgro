CREATE TABLE [dbo].[Log] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Fecha] DATETIME          NULL,
    [Xml]     NVARCHAR (mAX)         NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC),

);

