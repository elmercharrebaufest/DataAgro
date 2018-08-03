CREATE TABLE [dbo].[InformeComercialProduccion] (
    [InformeComerciaProduccionId] INT             IDENTITY (1, 1) NOT NULL,
    [InformeComercialId]          INT             NULL,
    [MaterialId]                  INT             NULL,
    [Hectareas]                   INT             NULL,
    [Toneladas]                   DECIMAL (18, 2) NULL,
    [LocalidadId]                 INT             NULL,
    [Propio]                      BIT             NULL,
    [Alquilado]                   BIT             NULL,
    [RtaOkSap]                    BIT             NULL,
    [MensajeSap]                  VARCHAR (500)   NULL,
    CONSTRAINT [PK_InformeComerciaDetalle] PRIMARY KEY CLUSTERED ([InformeComerciaProduccionId] ASC),
    CONSTRAINT [FK_InformeComerciaDetalle_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);



