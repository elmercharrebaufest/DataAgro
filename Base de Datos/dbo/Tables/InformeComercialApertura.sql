CREATE TABLE [dbo].[InformeComercialApertura]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [ComercialId] INT NOT NULL, 
    [FechaApertura] DATETIME NOT NULL,
    CONSTRAINT [PK_InformeComercialApertura] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE NONCLUSTERED INDEX [NDX_ComercialId] 
ON [dbo].[InformeComercialApertura] ([ComercialId]) 
INCLUDE ([Id],[FechaApertura])

GO