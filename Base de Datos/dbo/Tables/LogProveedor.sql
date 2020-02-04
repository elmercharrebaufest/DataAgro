CREATE TABLE [dbo].[LogProveedor]
(
	[LogProveedorId] INT   IDENTITY (1, 1) NOT NULL PRIMARY KEY,    
    [Fecha] DATETIME NOT NULL, 
    [ProveedorId] INT NOT NULL, 
    [ComercialId] INT NOT NULL,

	 CONSTRAINT [FK_LogProveedor_Comercial] FOREIGN KEY (ComercialId) REFERENCES Comercial(ComercialId), 
	 CONSTRAINT [FK_LogProveedor_Proveedor] FOREIGN KEY (ProveedorId) REFERENCES Proveedor(ProveedorId), 
 
 )