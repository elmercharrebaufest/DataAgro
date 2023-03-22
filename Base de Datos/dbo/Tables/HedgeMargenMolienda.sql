CREATE TABLE [dbo].[HedgeMargenMolienda] (
    [Id]					INT        IDENTITY (1, 1) NOT NULL,
    [MargenMolienda]		INT        NOT NULL,
    [Fecha]					DATETIME   NOT NULL,
    [ComercialId]			INT        NOT NULL

    CONSTRAINT [PK_HedgeMargenMolienda] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HedgeMargenMolienda_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);

