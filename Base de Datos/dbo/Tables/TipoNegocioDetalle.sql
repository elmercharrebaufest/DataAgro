CREATE TABLE [dbo].[TipoNegocioDetalle]
(
[Id] INT  IDENTITY (1, 1) NOT NULL,
[Descripcion] VARCHAR(100) NULL,
[TipoNegocioId] INT NULL,
BoletoFisico BIT NULL DEFAULT 0,
CartaOferta BIT NULL  DEFAULT 0,
Confirma BIT NULL  DEFAULT 0,
CONSTRAINT [PK_dbo.TipoNegocioDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
CONSTRAINT [FK_TipoNegocioDetalle_TipoNegocio] FOREIGN KEY ([TipoNegocioId]) REFERENCES [TipoNegocio]([TipoNegocioId]),
)

GO