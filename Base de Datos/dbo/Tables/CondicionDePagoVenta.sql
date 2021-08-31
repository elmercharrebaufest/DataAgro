CREATE TABLE [dbo].[CondicionDePagoVenta] (
    [Id] INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]      VARCHAR (150) NULL,
    [CondicionFijacion] BIT NULL, 
    [CondicionPesificado] BIT NULL, 
    CONSTRAINT [PK_CondicionDePagoVenta] PRIMARY KEY CLUSTERED ([Id] ASC)
);

