CREATE TABLE [dbo].[Condicion] (
    [CondicionId]  INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]  VARCHAR (20) NOT NULL,
    [Inhabilitado] BIT          NULL
);

