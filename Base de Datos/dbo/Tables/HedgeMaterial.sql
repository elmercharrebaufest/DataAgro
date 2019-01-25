CREATE TABLE [dbo].[HedgeMaterial] (
    [Id]					INT        IDENTITY (1, 1) NOT NULL,
    [MaterialId]			INT        NOT NULL,    
    [Cantidad]				DECIMAL (18, 2) NULL,
    [TipoHedgeMaterialId]   INT        NOT NULL,
    [Fecha]					DATETIME   NOT NULL,
	[ComercialId]			INT        NOT NULL

    CONSTRAINT [PK_HedgeMaterial] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_HedgeMaterial_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]), 
    CONSTRAINT [FK_HedgeMaterial_TipoHedgeMaterial] FOREIGN KEY (TipoHedgeMaterialId) REFERENCES [TipoHedgeMaterial]([Id]), 
    CONSTRAINT [FK_HedgeMaterial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);

