CREATE TABLE [dbo].[ClasificacionHabilitadoSinBoleto] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,	
    [ClasificacionId]         INT        NOT NULL,
    CONSTRAINT [PK_ClasificacionHabilitadoSinBoleto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClasificacionHabilitadoSinBoleto_Clasificacion] FOREIGN KEY ([ClasificacionId]) REFERENCES [dbo].[ClasificacionCompranet] ([Id])
);

