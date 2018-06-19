CREATE TABLE [dbo].[ComercialZona] (
    [ComercialZonaId] INT NOT NULL,
    [ComercialId]     INT NOT NULL,
    [NroItem]         INT NOT NULL,
    [ZonaId]          INT NOT NULL,
    CONSTRAINT [PK_ComercialZona] PRIMARY KEY CLUSTERED ([ComercialZonaId] ASC),
    CONSTRAINT [FK_ComercialZona_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
    CONSTRAINT [FK_ComercialZona_Zona] FOREIGN KEY ([ZonaId]) REFERENCES [dbo].[Zona] ([ZonaId])
);

