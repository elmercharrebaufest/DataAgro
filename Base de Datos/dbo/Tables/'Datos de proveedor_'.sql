CREATE TABLE [dbo].['Datos de proveedor$'] (
    [CUIT]                           FLOAT (53)     NULL,
    [Razón social]                   NVARCHAR (255) NULL,
    [Provincia]                      NVARCHAR (255) NULL,
    [Localidad]                      NVARCHAR (255) NULL,
    [Domicilio de actividad]         NVARCHAR (255) NULL,
    [Código postal]                  FLOAT (53)     NULL,
    [Canales de operación]           NVARCHAR (255) NULL,
    [Entrega a]                      NVARCHAR (255) NULL,
    [Condiciones preferentes]        NVARCHAR (255) NULL,
    [Intermediario]                  NVARCHAR (255) NULL,
    [Area de influencia -Zona única] NVARCHAR (255) NULL,
    [Area de influencia -Multizona]  NVARCHAR (255) NULL,
    [Comercial]                      NVARCHAR (255) NULL,
    [Id]                             INT            IDENTITY (1, 1) NOT NULL
);

