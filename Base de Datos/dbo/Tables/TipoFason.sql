CREATE TABLE [dbo].[TipoFason]
(
	[Id]					INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]				NVARCHAR(5) NOT NULL,	

    CONSTRAINT [PK_TipoFason] PRIMARY KEY CLUSTERED ([Id] ASC)
)
