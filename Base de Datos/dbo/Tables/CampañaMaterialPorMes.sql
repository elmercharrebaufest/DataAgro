CREATE TABLE [dbo].[CampañaMaterialPorMes] (
    [CampañaMaterialPorMesId] INT        NOT NULL,
    [NroItem]                 INT        NULL,
    [Mes]                     INT        NULL,
    [Toneladas]               FLOAT      NULL,
    [CampañaMaterialId]       INT        NULL,
    [Año]                     INT        NULL,
    [ComercialId]             INT        NULL,
    CONSTRAINT [PK_CampañaMaterialPorMes] PRIMARY KEY CLUSTERED ([CampañaMaterialPorMesId] ASC),
    CONSTRAINT [FK_CampañaMaterialPorMes_CampañaMaterial] FOREIGN KEY ([CampañaMaterialId]) REFERENCES [dbo].[CampañaMaterial] ([CampañaMaterialId])
);

