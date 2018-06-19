CREATE TABLE [dbo].[InformeComercialEstado] (
    [EstadoInformeId] INT           NOT NULL,
    [Descripcion]     VARCHAR (100) NULL,
    CONSTRAINT [PK_InformeComercialEstado] PRIMARY KEY CLUSTERED ([EstadoInformeId] ASC)
);

