CREATE TABLE [dbo].[GrupoDeComprasSAP] (
    [Id]          INT          NOT NULL,
    [GrupoCompraId] INT NULL,
    [NumeroSAP] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_GrupoDeComprasSAP] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_GrupoDeComprasSAP_GrupoCompras] FOREIGN KEY (GrupoCompraId) REFERENCES GrupoDeCompras([Id]) 
);

