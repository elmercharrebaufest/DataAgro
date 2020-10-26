CREATE TABLE [dbo].[HabilitacionCampaña] (
    [Id]					INT	IDENTITY (1, 1) NOT NULL,       
    [MaterialId]              INT NOT NULL, 
    [CampañaId]              INT NOT NULL,     
    CONSTRAINT [PK_HabilitacionCampaña] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HabilitacionCampaña_Material] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].Material ([MaterialId]),
	CONSTRAINT [FK_HabilitacionCampaña_Campaña] FOREIGN KEY ([CampañaId]) REFERENCES [dbo].Campaña ([CampañaId])

);

