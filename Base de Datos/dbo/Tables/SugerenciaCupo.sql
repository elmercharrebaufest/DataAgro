CREATE TABLE [dbo].[SugerenciaCupo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MaterialId] [int] NOT NULL,
	[CentroId] [int] NOT NULL,
	[FechaSugerida] [datetime] NOT NULL,
	[CantidadDeCupos] [int] NOT NULL,
	[TipoNegocioId] [int] NOT NULL,
 [Puntuacion] DECIMAL(9, 2) NOT NULL, 
    [MonedaId] CHAR(5) NULL, 
    [Precio] DECIMAL(9, 2) NULL, 
    [ProveedorId] INT NOT NULL, 
    [Aceptado] BIT NULL, 
    [StandardDeCalidad] VARCHAR(50) NOT NULL, 
    [ZonaCupoId] INT NOT NULL, 
    [ComercialId] INT NOT NULL, 
    [Destinatario] VARCHAR(50) NULL, 
    [Puntuaciones] VARCHAR(5000) NOT NULL, 
    [ContratoSAP] NVARCHAR(30) NULL, 
    [MotivoRechazo] VARCHAR(500) NULL, 
    [ConfiguracionEspacioDinamicoId] INT NULL, 
    [NegocioId] INT NULL, 
    CONSTRAINT [PK_SugerenciaCupo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Negocio] FOREIGN KEY([NegocioId])
REFERENCES [dbo].[Negocio] ([Id])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_Negocio]
GO


ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Material] FOREIGN KEY([MaterialId])
REFERENCES [dbo].[Material] ([MaterialId])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_Material]
GO

ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_TipoNegocio] FOREIGN KEY([TipoNegocioId])
REFERENCES [dbo].[TipoNegocio] ([TipoNegocioId])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_TipoNegocio]
GO

ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Moneda] FOREIGN KEY([MonedaId])
REFERENCES [dbo].[Moneda] ([MonedaId])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_Moneda]
GO

ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Proveedor] FOREIGN KEY([ProveedorId])
REFERENCES [dbo].[Proveedor] ([ProveedorId])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_Moneda]
GO

ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_ZonaCupo] FOREIGN KEY([ZonaCupoId])
REFERENCES [dbo].[ZonaCupo] ([Id])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_ZonaCupo]
GO

ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_Comercial] FOREIGN KEY([ComercialId])
REFERENCES [dbo].[Comercial] ([ComercialId])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_Comercial]
GO


ALTER TABLE [dbo].[SugerenciaCupo]  WITH CHECK ADD  CONSTRAINT [FK_SugerenciaCupo_ConfiguracionEspacioDinamico] FOREIGN KEY([ConfiguracionEspacioDinamicoId])
REFERENCES [dbo].[ConfiguracionEspacioDinamico] ([Id])
GO

ALTER TABLE [dbo].[SugerenciaCupo] CHECK CONSTRAINT [FK_SugerenciaCupo_ConfiguracionEspacioDinamico]
GO