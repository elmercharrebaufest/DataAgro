CREATE TABLE [dbo].[Comercial] (
    [ComercialId]       INT           IDENTITY (1, 1) NOT NULL,
    [Apellido]          VARCHAR (50)  NOT NULL,
    [Nombres]           VARCHAR (50)  NOT NULL,
    [PerfilId]          INT           NULL,
    [EmpleadorACargoId]   INT           NULL,
    [IdActiveDirectory] VARCHAR (100) NULL,
    [GrupoDeComprasId]    INT           NULL,
    [Administrador]     BIT           NULL,
	[Cupera]			BIT			NULL,
    [Deshabilitado] BIT NULL , 
    [FechaDeshabilitado] DATETIME NULL, 
    [AsignarNegocios] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Comercial] PRIMARY KEY CLUSTERED ([ComercialId] ASC),
    CONSTRAINT [FK_Comercial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
    CONSTRAINT [FK_Comercial_GrupoDeCompras] FOREIGN KEY ([GrupoDeComprasId]) REFERENCES [dbo].[GrupoDeCompras] ([Id]),
    CONSTRAINT [FK_Comercial_Perfil] FOREIGN KEY ([PerfilId]) REFERENCES [dbo].[Perfil] ([PerfilId])
);

