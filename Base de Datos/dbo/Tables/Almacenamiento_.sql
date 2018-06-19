CREATE TABLE [dbo].[Almacenamiento$] (
    [CUIT]                            FLOAT (53)     NULL,
    [Razón social]                    NVARCHAR (255) NULL,
    [Volumen anual total Tn]          FLOAT (53)     NULL,
    [Capacidad almacenamiento propia] FLOAT (53)     NULL,
    [Habilitado Sustentable]          NVARCHAR (255) NULL,
    [Provincia]                       NVARCHAR (255) NULL,
    [Localidad]                       NVARCHAR (255) NULL,
    [Arrendado]                       NVARCHAR (255) NULL,
    [Propio]                          NVARCHAR (255) NULL,
    [Campaña]                         NVARCHAR (255) NULL,
    [Grano]                           NVARCHAR (255) NULL,
    [Toneladas]                       FLOAT (53)     NULL,
    [Porcentaje]                      FLOAT (53)     NULL,
    [Comercial]                       NVARCHAR (255) NULL,
    [Id]                              INT            IDENTITY (1, 1) NOT NULL
);

