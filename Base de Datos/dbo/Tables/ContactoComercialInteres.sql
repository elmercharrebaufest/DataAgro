CREATE TABLE [dbo].[ContactoComercialInteres] (
    [ContactoComercialInteresId] INT IDENTITY (1, 1) NOT NULL,
    [ContactoComercialId]        INT NULL,
    [NroItem]                    INT NOT NULL,
    [InteresId]                  INT NOT NULL,
    CONSTRAINT [FK_ContactoComercialInteres_ContactoComercial] FOREIGN KEY ([ContactoComercialId]) REFERENCES [dbo].[ContactoComercial] ([ContactoComercialId]) ON DELETE SET NULL
);

