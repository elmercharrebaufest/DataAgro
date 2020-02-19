CREATE TABLE [dbo].[CampoMaterial] (
    [CampoMaterialid] INT IDENTITY (1, 1) NOT NULL,
    [CampoId]         INT NULL,
    [NroItem]         INT NOT NULL,
    [MaterialId]      INT NOT NULL,
    [Hectareas]       INT NULL,
    [Toneladas]       INT NULL,
    [CampañaId]       INT NOT NULL,
    CONSTRAINT [PK_CampoMaterial] PRIMARY KEY CLUSTERED ([CampoMaterialid] ASC),
    CONSTRAINT [FK_CampoMaterial_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
    CONSTRAINT [FK_CampoMaterial_Campo] FOREIGN KEY ([CampoId]) REFERENCES [dbo].[Campo] ([CampoId]) ON DELETE SET NULL,
    CONSTRAINT [FK_CampoMaterial_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

