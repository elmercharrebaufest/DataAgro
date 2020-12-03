CREATE TABLE [dbo].[SugerenciaPorComercial] (
    [Id] INT        IDENTITY (1, 1) NOT NULL,  
    [ComercialId]             INT        NULL,   
	[CentroId]          INT        NOT NULL,
    [MaterialId]         INT        NOT NULL,
	[Total] INT NULL, 
    [Fecha] DATETIME NULL, 
    CONSTRAINT [PK_SugerenciaPorComercial] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_SugerenciaPorComercial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId]),
	CONSTRAINT [FK_SugerenciaPorComercial_Centro] FOREIGN KEY ([CentroId]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_SugerenciaPorComercial_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([MaterialId])
);

