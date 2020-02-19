CREATE TABLE [dbo].[Objetivo] (
    [ObjetivoId]         INT        IDENTITY (1, 1) NOT NULL,
    [CampañaId]          INT        NULL,
    [MaterialId]         INT        NULL,
    [ToneladasObjetivos] FLOAT (53) NULL,
    [NroItem]            INT        NULL,
    [ProveedorId]        INT        NULL,
    CONSTRAINT [PK_Objetivo] PRIMARY KEY CLUSTERED ([ObjetivoId] ASC),
    CONSTRAINT [FK_Objetivo_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
    CONSTRAINT [FK_Objetivo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

