CREATE TABLE [dbo].[FijacionVirtualSAP] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [FijacionCanjeId]     INT           NOT NULL,
    [FijacionVirtualId]     INT       NULL,
    [FijacionVirtualNro] NVARCHAR(15)   NOT NULL,
    [Cantidad] FLOAT NOT NULL, 
    CONSTRAINT [PK_FijacionVirtualSAP] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FijacionVirtualSAP_FijacionCanjeId] FOREIGN KEY ([FijacionCanjeId]) REFERENCES [dbo].[Negocio] ([Id]),
    CONSTRAINT [FK_FijacionVirtualSAP_FijacionVirtualId] FOREIGN KEY ([FijacionVirtualId]) REFERENCES [dbo].[Negocio] ([Id]),
);

