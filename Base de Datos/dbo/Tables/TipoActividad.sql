CREATE TABLE [dbo].[TipoActividad] (
    [TipoActividadId] INT          NOT NULL,
    [Descripcion]     VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TipoActividad] PRIMARY KEY CLUSTERED ([TipoActividadId] ASC)
);

