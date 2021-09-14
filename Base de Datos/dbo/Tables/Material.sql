CREATE TABLE [dbo].[Material] (
    [MaterialId]    INT          IDENTITY (1, 1) NOT NULL,
    [Codigo]        VARCHAR (20) NOT NULL,
    [Descripcion]   VARCHAR (50) NOT NULL,
    [CampañaId]		INT          NULL,
    [CampaniaTableroId] INT NULL, 
	[CodigoEspecie]	INT	NULL,   
    [IVA] DECIMAL(11, 2) NULL, 
    CONSTRAINT [PK_Material] PRIMARY KEY CLUSTERED ([MaterialId] ASC),
    CONSTRAINT [FK_Material_Campania] FOREIGN KEY ([CampaniaTableroId]) REFERENCES [dbo].[Campaña] ([CampañaId]),
	CONSTRAINT [FK_Material_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].[Campaña] ([CampañaId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Material]
    ON [dbo].[Material]([Codigo] ASC);

