CREATE TABLE [dbo].[Destinatario] (
    [DestinatarioId] INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]    VARCHAR (50) NOT NULL,
    [Inhabilitado]   BIT          NULL,
    CONSTRAINT [PK_Destinatario] PRIMARY KEY CLUSTERED ([DestinatarioId] ASC)
);

