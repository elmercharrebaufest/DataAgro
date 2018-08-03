CREATE TABLE [dbo].[InformeComercialEstado] (
    [EstadoInformeId] INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion]     VARCHAR (100) NULL,
    CONSTRAINT [PK_InformeComercialEstado] PRIMARY KEY CLUSTERED ([EstadoInformeId] ASC)
);

