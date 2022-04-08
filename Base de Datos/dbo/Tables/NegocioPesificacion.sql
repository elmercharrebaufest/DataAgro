CREATE TABLE [dbo].[NegocioPesificacion] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Excepcion]          BIT           NULL,
    [FechaExcepcion]       DATETIME      NULL,
    [FechaInstruccion]    DATETIME      NULL,
    [FechaEnvio]    DATETIME      NULL,
    [ComercialId]              INT           NULL,
    [NegocioId]      INT           NULL,
    CONSTRAINT [PK_NegocioPesificacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NegocioPesificacion_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
    CONSTRAINT [FK_NegocioPesificacion_Negocio] FOREIGN KEY ([NegocioId]) REFERENCES [dbo].[Negocio] ([Id]),
);

