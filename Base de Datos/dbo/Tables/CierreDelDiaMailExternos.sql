CREATE TABLE [dbo].[CierreDelDiaMailExternos] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [Mail] VARCHAR(1500) NOT NULL, 
    CONSTRAINT [PK_CierreDelDiaMailExternos] PRIMARY KEY CLUSTERED ([Id] ASC),
);

