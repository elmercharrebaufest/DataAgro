CREATE TABLE [dbo].[FechaFeriado] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Feriado]     DATE           NOT NULL,    
    CONSTRAINT [PK_FechaFeriado] PRIMARY KEY CLUSTERED ([Id] ASC)
);

