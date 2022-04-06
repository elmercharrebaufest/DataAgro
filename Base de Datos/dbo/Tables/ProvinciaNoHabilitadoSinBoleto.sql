CREATE TABLE [dbo].[ProvinciaNoHabilitadoSinBoleto] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,	
    [ProvinciaId]         INT        NOT NULL,
    CONSTRAINT [PK_ProvinciaNoHabilitadoSinBoleto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProvinciaNoHabilitadoSinBoleto_Provincia] FOREIGN KEY ([ProvinciaId]) REFERENCES [dbo].[Provincia] ([ProvinciaId])
);

