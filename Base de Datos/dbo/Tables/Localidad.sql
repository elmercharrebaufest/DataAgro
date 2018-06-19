CREATE TABLE [dbo].[Localidad] (
    [LocalidadId]  INT           NOT NULL,
    [CodLocalidad] VARCHAR (10)  NOT NULL,
    [Nombre]       VARCHAR (100) NOT NULL,
    [ProvinciaId]  INT           NOT NULL,
    CONSTRAINT [PK_Localidad] PRIMARY KEY CLUSTERED ([LocalidadId] ASC),
    CONSTRAINT [FK_Localidad_Provincia] FOREIGN KEY ([ProvinciaId]) REFERENCES [dbo].[Provincia] ([ProvinciaId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Localidad]
    ON [dbo].[Localidad]([CodLocalidad] ASC);

