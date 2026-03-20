-- Tabla de auditoría para ContactoComercial
CREATE TABLE [dbo].[ContactoComercialAuditoria] (
    [ContactoComercialAuditoriaId] INT PRIMARY KEY IDENTITY(1,1),
    [ContactoComercialId] INT NOT NULL,
    [ProveedorId] INT,
    [Apellido] NVARCHAR(MAX),
    [Nombres] NVARCHAR(MAX),
    [CuitApoderado] NVARCHAR(MAX),
    [FechaDesde] DATETIME2,
    [FechaHasta] DATETIME2,
    [EsApoderado] BIT,
    [PuestoApoderadoId] INT,
    [Puesto] NVARCHAR(MAX),
    [Telefono1] NVARCHAR(MAX),
    [TipoTelefono1Id] INT,
    [Telefono2] NVARCHAR(MAX),
    [TipoTelefono2Id] INT,
    [Telefono3] NVARCHAR(MAX),
    [TipoTelefono3Id] INT,
    [Email1] NVARCHAR(MAX),
    [Email2] NVARCHAR(MAX),
    [Email3] NVARCHAR(MAX),
    [FechaNacimiento] DATETIME2,
    [OtrosIntereses] NVARCHAR(MAX),
    [EsPrincipal] BIT,
    [Cargo] NVARCHAR(MAX),
    [CompraNet] BIT,
    [Cupo] BIT,
    [Boleto] BIT,
    [TipoOperacion] NVARCHAR(10) NOT NULL, -- 'INSERT', 'UPDATE', 'DELETE'
    [UsuarioModificacion] NVARCHAR(MAX),
    [FechaModificacion] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ValoresAnteriores] NVARCHAR(MAX), -- JSON con valores anteriores en UPDATE
    [ValoresNuevos] NVARCHAR(MAX) -- JSON con valores nuevos en INSERT/UPDATE
);
-- Índice para búsquedas rápidas
GO
CREATE INDEX [IX_ContactoComercialAuditoria_ContactoComercialId] 
ON [dbo].[ContactoComercialAuditoria]([ContactoComercialId]);

GO
CREATE INDEX [IX_ContactoComercialAuditoria_FechaModificacion] 
ON [dbo].[ContactoComercialAuditoria]([FechaModificacion]);

GO
