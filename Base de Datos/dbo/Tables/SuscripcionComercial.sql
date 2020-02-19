CREATE TABLE [dbo].[SuscripcionComercial] (
	[Id]				INT IDENTITY(1,1)	NOT NULL,
    [ComercialId]       INT					NOT NULL,
    [Key]				VARCHAR(MAX)			NOT NULL
    CONSTRAINT [PK_SuscripcionComercial] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SuscripcionComercial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [dbo].[Comercial] ([ComercialId])
);

