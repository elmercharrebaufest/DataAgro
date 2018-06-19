CREATE TABLE [dbo].['Datos de contacto$'] (
    [CUIT]             FLOAT (53)     NULL,
    [Razón social]     NVARCHAR (255) NULL,
    [Nombre referente] NVARCHAR (255) NULL,
    [Mail]             NVARCHAR (255) NULL,
    [Teléfono]         NVARCHAR (255) NULL,
    [Calificación]     FLOAT (53)     NULL,
    [Comentarios]      NVARCHAR (255) NULL,
    [Comercial]        NVARCHAR (255) NULL,
    [Id]               INT            IDENTITY (1, 1) NOT NULL
);

