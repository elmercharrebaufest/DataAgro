CREATE TABLE [dbo].[Errores] (
    [ErrorId]         INT           IDENTITY (1, 1) NOT NULL,
    [ErrorDateTime]   DATETIME      NULL,
    [MachineName]     VARCHAR (50)  NULL,
    [AppDomainName]   VARCHAR (50)  NULL,
    [ThreadIdentity]  VARCHAR (50)  NULL,
    [WindowsIdentity] VARCHAR (50)  NULL,
    [Message]         VARCHAR (200) NULL,
    [FullException]   TEXT          NULL,
    CONSTRAINT [PK_Errores] PRIMARY KEY CLUSTERED ([ErrorId] ASC)
);

