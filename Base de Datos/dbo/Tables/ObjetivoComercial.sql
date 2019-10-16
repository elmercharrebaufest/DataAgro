CREATE TABLE [dbo].[ObjetivoComercial] (
    [Id]					INT			IDENTITY (1, 1) NOT NULL,
    [CampanaId]				INT			NULL,
    [MaterialId]			INT			NULL,
	[ComercialId]			INT			NULL,
    [ToneladasObjetivos]	FLOAT (53)	NULL,
    CONSTRAINT [PK_ObjetivoComercial] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ObjetivoComercial_Campana] FOREIGN KEY ([CampanaId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
    CONSTRAINT [FK_ObjetivoComercial_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId]),
    CONSTRAINT [FK_ObjetivoComercial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId])
);

