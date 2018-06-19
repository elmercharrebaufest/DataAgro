CREATE TABLE [dbo].[CampañaMaterialHistorico] (
    [CampañaMaterialHistoricoId] INT      NOT NULL,
    [CampañaId]                  INT      NOT NULL,
    [MaterialId]                 INT      NOT NULL,
    [Fecha]                      DATETIME NOT NULL,
    CONSTRAINT [PK_CampañaMaterialHistorico] PRIMARY KEY CLUSTERED ([CampañaMaterialHistoricoId] ASC)
);

