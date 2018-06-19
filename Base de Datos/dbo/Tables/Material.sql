CREATE TABLE [dbo].[Material] (
    [MaterialId]      INT          NOT NULL,
    [Codigo]          VARCHAR (20) NOT NULL,
    [Descripcion]     VARCHAR (50) NOT NULL,
    [CampañaIdActual] INT          NULL,
    CONSTRAINT [PK_Material] PRIMARY KEY CLUSTERED ([MaterialId] ASC),
    CONSTRAINT [FK_Material_Campaña] FOREIGN KEY ([CampañaIdActual]) REFERENCES [dbo].[Campaña] ([CampañaId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Material]
    ON [dbo].[Material]([Codigo] ASC);

