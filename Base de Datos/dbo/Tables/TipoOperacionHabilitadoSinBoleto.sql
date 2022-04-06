CREATE TABLE [dbo].[TipoOperacionHabilitadoSinBoleto] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,	
    [Directo]         BIT        NULL,
    [Corredor] BIT NULL, 
    CONSTRAINT [PK_TipoOperacionHabilitadoSinBoleto] PRIMARY KEY CLUSTERED ([Id] ASC),
);

