CREATE TABLE [dbo].[AcopioMaterial] (
    [AcopioMaterialId] INT        IDENTITY (1, 1) NOT NULL,
    [AcopioId]         INT        NULL,
    [NroItem]          INT        NOT NULL,
    [Toneladas]        FLOAT	  NULL,
    [CampañaId]        INT        NOT NULL,
    [MaterialId]       INT        NOT NULL,
    CONSTRAINT [PK_AcopioMaterial] PRIMARY KEY CLUSTERED ([AcopioMaterialId] ASC)
);

