CREATE TABLE [dbo].[TipoActividad] (
    [TipoActividadId] INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]     VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TipoActividad] PRIMARY KEY CLUSTERED ([TipoActividadId] ASC)
);

