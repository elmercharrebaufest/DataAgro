CREATE TABLE [dbo].[AreaInfluencia] (
    [AreaInfluenciaId] INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]      VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_AreaInfluencia] PRIMARY KEY CLUSTERED ([AreaInfluenciaId] ASC)
);

