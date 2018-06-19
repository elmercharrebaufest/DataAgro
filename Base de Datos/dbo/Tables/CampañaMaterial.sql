CREATE TABLE [dbo].[CampañaMaterial] (
    [CampañaMaterialId]  INT        NOT NULL,
    [CampañaId]          INT        NOT NULL,
    [NroItem]            INT        NOT NULL,
    [ProveedorId]        INT        NOT NULL,
    [MaterialId]         INT        NOT NULL,
    [ToneladasCompradas] FLOAT (53) NOT NULL,
    CONSTRAINT [PK_CampañaMaterial] PRIMARY KEY CLUSTERED ([CampañaMaterialId] ASC),
    CONSTRAINT [FK_CampañaMaterial_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
    CONSTRAINT [FK_CampañaMaterial_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_CampañaMaterial_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

