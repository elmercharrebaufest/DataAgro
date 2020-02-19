CREATE TABLE [dbo].[Diferencial] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [DiferencialDefault]          INT	NOT NULL,
	[Fecha]			DATETIME NOT NULL,
	[ComercialId]			INT        NOT NULL
	
    CONSTRAINT [PK_Diferencial] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Diferencial_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);

