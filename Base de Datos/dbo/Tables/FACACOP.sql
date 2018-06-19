CREATE TABLE [dbo].[FACACOP] (
    [Id]                      INT           NOT NULL,
    [CUIT]                    CHAR (11)     NOT NULL,
    [Fecha1]                  DATETIME      NOT NULL,
    [Fecha2]                  DATETIME      NOT NULL,
    [ObservacionesEspeciales] VARCHAR (500) NULL,
    CONSTRAINT [PK_FACACOP] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [CUIT]
    ON [dbo].[FACACOP]([CUIT] ASC);

