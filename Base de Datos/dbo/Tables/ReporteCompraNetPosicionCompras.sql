CREATE TABLE [dbo].[ReporteCompraNetPosicionCompras] (
 [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [Material]			VARCHAR(50) NOT NULL,
    [MaterialId]         int NOT NULL,
    [Mes]         VARCHAR(50) NOT NULL,
    [KilosPesos]         DECIMAL(11, 2) NOT NULL,
    [KilosDolares]         DECIMAL(11, 2) NOT NULL,
    [DispAFijar]         DECIMAL(11, 2) NOT NULL,
    [DispAPrecio]         DECIMAL(11, 2) NOT NULL,
    [DispFijac]         DECIMAL(11, 2) NOT NULL,
    [FrwAFijar]         DECIMAL(11, 2) NOT NULL,
    [FrwAPrecio]         DECIMAL(11, 2) NOT NULL,
    [FrwFijac]         DECIMAL(11, 2) NOT NULL,
    [NewAFijar]         DECIMAL(11, 2) NOT NULL,
    [NewAPrecio]         DECIMAL(11, 2) NOT NULL,
    [NewFijac]         DECIMAL(11, 2) NOT NULL,
    [Anio]         int,
    [PrecioPonderadoPesos]			DECIMAL(11, 2) ,
    [PrecioPonderadoDolares]			DECIMAL(11, 2) ,
    [CantidadPonderada]			DECIMAL(11, 2) ,
	[DispAPrecioPesos]			DECIMAL(11, 2) NOT NULL default 0,
	[DispAPrecioDolares]			DECIMAL(11, 2) NOT NULL default 0,
	[DispFijacPesos]			DECIMAL(11, 2) NOT NULL default 0,
	[DispFijacDolares]			DECIMAL(11, 2) NOT NULL default 0,
	[FrwAPrecioPesos]			DECIMAL(11, 2) NOT NULL default 0,
	[FrwAPrecioDolares]			DECIMAL(11, 2) NOT NULL default 0,
	[FrwFijacPesos]			DECIMAL(11, 2) NOT NULL default 0,
	[FrwFijacDolares]			DECIMAL(11, 2) NOT NULL default 0,
	[NewAPrecioPesos]			DECIMAL(11, 2) NOT NULL default 0,
	[NewAPrecioDolares]			DECIMAL(11, 2) NOT NULL default 0,
	[NewFijacPesos]			DECIMAL(11, 2) NOT NULL default 0,
	[NewFijacDolares]			DECIMAL(11, 2) NOT NULL default 0,
	    [CampanaId] INT NULL, 
    [Campana] VARCHAR(50) NULL, 
    CONSTRAINT [PK_ReporteCompraNetPosicionCompras] PRIMARY KEY CLUSTERED ([Id] ASC)
);

--go

--CREATE TRIGGER InsteadReporteCompraNetPosicionCompras on ReporteCompraNetPosicionCompras INSTEAD OF INSERT AS
--BEGIN
--    BEGIN TRANSACTION

 

--        delete gslosqlprd00.moa_siogranos.dbo.ReporteCompraNetPosicionCompras

 

--        INSERT INTO gslosqlprd00.moa_siogranos.dbo.ReporteCompraNetPosicionCompras (Material,MaterialId,Mes,KilosPesos,KilosDolares,DispAFijar,DispAPrecio,DispFijac,FrwAFijar,FrwAPrecio,FrwFijac,NewAFijar,NewAPrecio,NewFijac,Anio,PrecioPonderadoPesos,PrecioPonderadoDolares,CantidadPonderada,DispAPrecioPesos,DispAPrecioDolares,DispFijacPesos,DispFijacDolares,FrwAPrecioPesos,FrwAPrecioDolares,FrwFijacPesos,FrwFijacDolares,NewAPrecioPesos,NewAPrecioDolares,NewFijacPesos,NewFijacDolares)
--        SELECT Material,MaterialId,Mes,KilosPesos,KilosDolares,DispAFijar,DispAPrecio,DispFijac,FrwAFijar,FrwAPrecio,FrwFijac,NewAFijar,NewAPrecio,NewFijac,Anio,PrecioPonderadoPesos,PrecioPonderadoDolares,CantidadPonderada,DispAPrecioPesos,DispAPrecioDolares,DispFijacPesos,DispFijacDolares,FrwAPrecioPesos,FrwAPrecioDolares,FrwFijacPesos,FrwFijacDolares,NewAPrecioPesos,NewAPrecioDolares,NewFijacPesos,NewFijacDolares
--        FROM ReporteCompraNetPosicionCompras

 

--        INSERT INTO gslosqlprd00.moa_siogranos.dbo.ReporteCompraNetPosicionCompras (Material,MaterialId,Mes,KilosPesos,KilosDolares,DispAFijar,DispAPrecio,DispFijac,FrwAFijar,FrwAPrecio,FrwFijac,NewAFijar,NewAPrecio,NewFijac,Anio,PrecioPonderadoPesos,PrecioPonderadoDolares,CantidadPonderada,DispAPrecioPesos,DispAPrecioDolares,DispFijacPesos,DispFijacDolares,FrwAPrecioPesos,FrwAPrecioDolares,FrwFijacPesos,FrwFijacDolares,NewAPrecioPesos,NewAPrecioDolares,NewFijacPesos,NewFijacDolares)
--        SELECT Material,MaterialId,Mes,KilosPesos,KilosDolares,DispAFijar,DispAPrecio,DispFijac,FrwAFijar,FrwAPrecio,FrwFijac,NewAFijar,NewAPrecio,NewFijac,Anio,PrecioPonderadoPesos,PrecioPonderadoDolares,CantidadPonderada,DispAPrecioPesos,DispAPrecioDolares,DispFijacPesos,DispFijacDolares,FrwAPrecioPesos,FrwAPrecioDolares,FrwFijacPesos,FrwFijacDolares,NewAPrecioPesos,NewAPrecioDolares,NewFijacPesos,NewFijacDolares
--        FROM inserted

 

--    COMMIT TRANSACTION

 

--END
GO