CREATE TABLE [dbo].[ProveedorEstado] (
    [ProveedorEstadoId] INT NOT NULL,
    [ProveedorId]       INT NULL,
    [EstadoId]          INT NULL,
    [ComercialId]       INT NULL,
    CONSTRAINT [PK_ProveedorEstado] PRIMARY KEY CLUSTERED ([ProveedorEstadoId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [ProveedorEstado]
    ON [dbo].[ProveedorEstado]([ProveedorId] ASC, [EstadoId] ASC);

