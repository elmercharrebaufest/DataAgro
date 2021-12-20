CREATE TABLE [dbo].[MensajeCupo]
(
	[Id]					INT NOT NULL IDENTITY (1, 1),    
    [ComercialId]			INT NULL,    
    [Datos] NVARCHAR(MAX) NULL, 
    [Fecha] DATETIME NULL, 
    [MaterialId] INT NULL, 
    [CentroId] INT NULL, 
    CONSTRAINT [PK_MensajeCupo] PRIMARY KEY CLUSTERED ([Id] ASC),	
    CONSTRAINT [FK_MensajeCupo_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId]),     
    CONSTRAINT [FK_MensajeCupo_Material] FOREIGN KEY ([MaterialId]) REFERENCES [Material]([MaterialId]),   
	CONSTRAINT [FK_MensajeCupo_Centro] FOREIGN KEY ([CentroId]) REFERENCES [Centro]([Id]),   
)
