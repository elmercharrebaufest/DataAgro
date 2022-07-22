CREATE TABLE [dbo].[Objetivo] (
    [ObjetivoId]         INT        IDENTITY (1, 1) NOT NULL,
    [CampañaId]          INT        NULL,
    [MaterialId]         INT        NULL,
    [ToneladasObjetivos] FLOAT (53) NULL,
    [NroItem]            INT        NULL,
    [ProveedorId]        INT        NULL,
    [ComercialId] INT NULL, 
    [GrupoDeComprasId] INT NULL, 
    CONSTRAINT [PK_Objetivo] PRIMARY KEY CLUSTERED ([ObjetivoId] ASC),
    CONSTRAINT [FK_Objetivo_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
    CONSTRAINT [FK_Objetivo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
    CONSTRAINT [FK_Objetivo_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
    CONSTRAINT [FK_Objetivo_GrupoDeCompras] FOREIGN KEY ([GrupoDeComprasId]) REFERENCES [dbo].[GrupoDeCompras] ([Id])
);

