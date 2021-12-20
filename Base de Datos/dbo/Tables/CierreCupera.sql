CREATE TABLE [dbo].[CierreCupera] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,	
    [MaterialId]         INT        NOT NULL,
    [Cierre] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_CierreCupera] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CierreCupera_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

