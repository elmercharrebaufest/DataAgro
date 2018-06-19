CREATE TABLE [dbo].[InformeComercialAlmacenamiento] (
    [InformeComercialAlmacenamientoId] INT             NOT NULL,
    [InformeComercialId]               INT             NULL,
    [Toneladas]                        DECIMAL (18, 2) NULL,
    [LocalidadId]                      INT             NULL,
    [Propia]                           BIT             NULL,
    [Alquilada]                        BIT             NULL,
    CONSTRAINT [PK_InformeComercialAlmacenamiento] PRIMARY KEY CLUSTERED ([InformeComercialAlmacenamientoId] ASC)
);



