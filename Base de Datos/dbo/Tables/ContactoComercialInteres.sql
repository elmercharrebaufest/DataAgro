CREATE TABLE [dbo].[ContactoComercialInteres] (
    [ContactoComercialInteresId] INT NOT NULL,
    [ContactoComercialId]        INT NULL,
    [NroItem]                    INT NOT NULL,
    [InteresId]                  INT NOT NULL,
    CONSTRAINT [FK_ContactoComercialInteres_ContactoComercial] FOREIGN KEY ([ContactoComercialId]) REFERENCES [dbo].[ContactoComercial] ([ContactoComercialId]) ON DELETE SET NULL
);

