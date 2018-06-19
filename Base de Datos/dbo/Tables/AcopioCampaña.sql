CREATE TABLE [dbo].[AcopioCampaña] (
    [AcopioCampañaId] INT        NOT NULL,
    [AcopioId]        INT        NOT NULL,
    [NroItem]         INT        NOT NULL,
    [Toneladas]       FLOAT (53) NULL,
    [CampañaId]       INT        NOT NULL,
    [HasArrendadas]   BIT        NULL,
    CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED ([AcopioCampañaId] ASC)
);

