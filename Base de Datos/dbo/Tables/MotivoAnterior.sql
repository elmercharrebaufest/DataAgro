CREATE TABLE [dbo].[MotivoAnterior](
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    
    CONSTRAINT [PK_dbo.MotivoAnterior] PRIMARY KEY CLUSTERED ([Id] ASC)
);
