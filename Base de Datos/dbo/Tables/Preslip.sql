CREATE TABLE [dbo].[Preslip] (
    [PreslipId]       INT             NOT NULL,
    [TipoDeNegocioId] INT             NULL,
    [MaterialId]      INT             NULL,
    [CampañaId]       INT             NULL,
    [Cantidad]        DECIMAL (18, 2) NULL,
    [Precio]          DECIMAL (18, 2) NULL,
    [FechaDesde]      DATETIME        NULL,
    [FechaHasta]      DATETIME        NULL,
    [FechaDeEntrega]  DATETIME        NULL,
    [ProveedorId]     INT             NULL,
    [EstadoPreslipId] INT             NULL,
    CONSTRAINT [PK_Preslip] PRIMARY KEY CLUSTERED ([PreslipId] ASC)
);

