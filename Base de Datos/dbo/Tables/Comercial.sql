CREATE TABLE [dbo].[Comercial] (
    [ComercialId]       INT           NOT NULL,
    [Apellido]          VARCHAR (50)  NOT NULL,
    [Nombres]           VARCHAR (50)  NOT NULL,
    [PerfilId]          INT           NOT NULL,
    [EmpleadorACargo]   INT           NULL,
    [IdActiveDirectory] VARCHAR (100) NULL,
    [GrupoDeCompras]    INT           NULL,
    [Administrador]     BIT           NULL,
    CONSTRAINT [PK_Comercial] PRIMARY KEY CLUSTERED ([ComercialId] ASC),
    CONSTRAINT [FK_Comercial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId]),
    CONSTRAINT [FK_Comercial_GrupoDeCompras] FOREIGN KEY ([GrupoDeCompras]) REFERENCES [dbo].[GrupoDeCompras] ([Id]),
    CONSTRAINT [FK_Comercial_Perfil] FOREIGN KEY ([PerfilId]) REFERENCES [dbo].[Perfil] ([PerfilId])
);

