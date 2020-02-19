CREATE TABLE [dbo].[HedgeTc] (
    [Id]					INT        IDENTITY (1, 1) NOT NULL,
    [TipoCambio]			DECIMAL (18, 2) NOT NULL,
    [HedgePesos]			DECIMAL (18, 2) NOT NULL,
    [Fecha]					DATETIME   NOT NULL,
	[ComercialId]			INT        NOT NULL

    CONSTRAINT [PK_HedgeTC] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_HedgeTC_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);

