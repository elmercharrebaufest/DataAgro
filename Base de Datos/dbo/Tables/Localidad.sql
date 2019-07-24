CREATE TABLE [dbo].[Localidad] (
    [LocalidadId]  INT           IDENTITY (1, 1) NOT NULL,
    [CodLocalidad] VARCHAR (10)  NOT NULL,
    [Nombre]       VARCHAR (100) NOT NULL,
    [ProvinciaId]  INT           NOT NULL,
	[PartidoId]  INT            NULL,
    CONSTRAINT [PK_Localidad] PRIMARY KEY CLUSTERED ([LocalidadId] ASC),
    CONSTRAINT [FK_Localidad_Provincia] FOREIGN KEY ([ProvinciaId]) REFERENCES [dbo].[Provincia] ([ProvinciaId]),
	CONSTRAINT [FK_Localidad_Partido] FOREIGN KEY ([PartidoId]) REFERENCES [dbo].[Partido] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Localidad]
    ON [dbo].[Localidad]([CodLocalidad] ASC);

