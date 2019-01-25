CREATE TABLE [dbo].[FinDelDia] (
    [Id]		INT         IDENTITY (1, 1) NOT NULL,
    [Dia]		DATETIME  NOT NULL,
	[Cerrado]	BIT NOT NULL,
	[ComercialId]INT NOT NULL
    CONSTRAINT [PK_FinDelDia] PRIMARY KEY CLUSTERED ([Id] ASC)
    CONSTRAINT [FK_FinDelDia_Comercial] FOREIGN KEY ([ComercialId]) REFERENCES [Comercial]([ComercialId])
);
