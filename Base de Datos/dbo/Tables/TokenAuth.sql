CREATE TABLE [dbo].[TokenAuth]
(
	[Cuit]					bigint NOT NULL,
    [Token]				NVARCHAR(500) NOT NULL,	
	[Vencimiento]       datetime not null,
	[NombreUsuario]		NVARCHAR(50) NOT NULL,	
    CONSTRAINT [PK_TokenAuth] PRIMARY KEY CLUSTERED ([Cuit] ASC)
)
