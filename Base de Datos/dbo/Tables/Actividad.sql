CREATE TABLE [dbo].[Actividad] (
    [ActividadId]              INT           IDENTITY (1, 1) NOT NULL,
    [TipoActividadId]          INT           NOT NULL,
    [Detalle]                  VARCHAR(MAX)  NOT NULL,
    [ProveedorId]              INT           NOT NULL,
    [FechaHoraActividad]       DATETIME      NOT NULL,
    [FechaHoraRecordatorio]    DATETIME      NULL,
    [ComercialId]              INT           NULL,
    [ContactoComercialId]      INT           NULL,
    [FechaHoraRecordatorioFin] DATETIME      NULL,
    [asunto]                   VARCHAR (100) NULL,
    CONSTRAINT [PK_Actividad] PRIMARY KEY CLUSTERED ([ActividadId] ASC),
    CONSTRAINT [FK_Actividad_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
    CONSTRAINT [FK_Actividad_Contacto] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [FK_Actividad_ContactoComercial] FOREIGN KEY ([ContactoComercialId]) REFERENCES [dbo].[ContactoComercial] ([ContactoComercialId]),
    CONSTRAINT [FK_Actividad_TipoActividad] FOREIGN KEY ([TipoActividadId]) REFERENCES [dbo].[TipoActividad] ([TipoActividadId])
);

