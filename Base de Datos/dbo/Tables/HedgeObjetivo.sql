CREATE TABLE [dbo].[HedgeObjetivo] (
    [Id]					INT        IDENTITY (1, 1) NOT NULL,
    [MaterialId]			INT        NOT NULL,    
    [Cantidad]				DECIMAL (18, 2) NULL,
    [TipoObjetivoId]		INT        NOT NULL,
    [Fecha]					DATETIME   NOT NULL,
	[ComercialId]			INT        NOT NULL

    CONSTRAINT [PK_HedgeObjetivo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_HedgeObjetivo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]), 
    CONSTRAINT [FK_HedgeObjetivo_TipoHedgeMaterial] FOREIGN KEY (TipoObjetivoId) REFERENCES [TipoObjetivo]([Id]), 
    CONSTRAINT [FK_HedgeObjetivo_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);

