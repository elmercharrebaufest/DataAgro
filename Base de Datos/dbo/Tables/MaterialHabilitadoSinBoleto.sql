CREATE TABLE [dbo].[MaterialHabilitadoSinBoleto] (
    [Id]  INT        IDENTITY (1, 1) NOT NULL,	
    [MaterialId]         INT        NOT NULL,
    CONSTRAINT [PK_MaterialHabilitadoSinBoleto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MaterialHabilitadoSinBoleto_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

