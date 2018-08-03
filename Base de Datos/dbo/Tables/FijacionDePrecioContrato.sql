CREATE TABLE [dbo].[FijacionDePrecioContrato]
(
	[FijacionDePrecioContratoId] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[ContratoId] INT NULL, 
	[ProveedorId] INT NOT NULL,
    [MaterialId] INT NULL,
	[MonedaId] CHAR(5) NOT NULL, 
    [ComercialId] INT NOT NULL, 
    [Precio] DECIMAL(18, 2) NOT NULL, 
    [Cantidad] INT NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [Ampliaciones] INT NULL, 
    [EstadoId] INT NOT NULL, 
    [Observacion] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_FijacionDePrecioContrato_Material] FOREIGN KEY (MaterialId) REFERENCES [Material]([MaterialId]), 
    CONSTRAINT [FK_FijacionDePrecioContrato_Moneda] FOREIGN KEY (MonedaId) REFERENCES Moneda(MonedaId), 
    CONSTRAINT [FK_FijacionDePrecioContrato_Comercial] FOREIGN KEY (ComercialId) REFERENCES Comercial(ComercialId), 
    CONSTRAINT [FK_FijacionDePrecioContrato_ToTable] FOREIGN KEY (ProveedorId) REFERENCES Proveedor(ProveedorId),
	CONSTRAINT [FK_FijacionDePrecioContrato_EstadoContrato] FOREIGN KEY (EstadoId) REFERENCES [EstadoContrato]([EstadoContratoId]),
   
)
