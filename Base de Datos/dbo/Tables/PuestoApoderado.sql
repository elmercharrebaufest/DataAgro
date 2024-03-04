CREATE TABLE [dbo].[PuestoApoderado] (
    [PuestoApoderadoId]    INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]        VARCHAR (20) NULL, 
    CONSTRAINT [PK_PuestoApoderado] PRIMARY KEY ([PuestoApoderadoId]),
);
