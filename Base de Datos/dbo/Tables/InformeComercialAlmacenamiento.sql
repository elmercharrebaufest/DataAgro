CREATE TABLE [dbo].[InformeComercialAlmacenamiento] (
    [InformeComercialAlmacenamientoId] INT             IDENTITY (1, 1) NOT NULL,
    [InformeComercialId]               INT             NULL,
    [Toneladas]                        DECIMAL (18, 2) NULL,
    [LocalidadId]                      INT             NULL,
    [Propia]                           BIT             NULL,
    [Alquilada]                        BIT             NULL,
    CONSTRAINT [PK_InformeComercialAlmacenamiento] PRIMARY KEY CLUSTERED ([InformeComercialAlmacenamientoId] ASC),
    CONSTRAINT [FK_InformeComercialAlmacenamiento_InformeComercial] FOREIGN KEY ([InformeComercialId]) REFERENCES [dbo].[InformeComercial] ([InformeComercialId])
);



