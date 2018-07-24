CREATE TABLE [dbo].[ClasificacionCompraNet]
(
	 [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    
    CONSTRAINT [PK_dbo.ClasificacionCompraNet] PRIMARY KEY CLUSTERED ([Id] ASC)
)
