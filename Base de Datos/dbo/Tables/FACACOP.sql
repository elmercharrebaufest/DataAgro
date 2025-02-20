CREATE TABLE [dbo].[FACACOP] (
    [Id]                      INT           IDENTITY (1, 1) NOT NULL,
    [CUIT]                    NVARCHAR (11) NOT NULL,
    [Fecha1]                  DATETIME      NOT NULL,
    [Fecha2]                  DATETIME      NOT NULL,
    [ObservacionesEspeciales] VARCHAR (500) NULL,
    CONSTRAINT [PK_FACACOP] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [CUIT]
    ON [dbo].[FACACOP]([CUIT] ASC);

