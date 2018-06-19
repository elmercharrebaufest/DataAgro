CREATE TABLE [dbo].[AcopioMaterial] (
    [AcopioMaterialId] INT        NOT NULL,
    [AcopioId]         INT        NULL,
    [NroItem]          INT        NOT NULL,
    [Toneladas]        FLOAT (53) NULL,
    [CampañaId]        INT        NOT NULL,
    [MaterialId]       INT        NOT NULL,
    CONSTRAINT [PK_AcopioMaterial] PRIMARY KEY CLUSTERED ([AcopioMaterialId] ASC)
);

