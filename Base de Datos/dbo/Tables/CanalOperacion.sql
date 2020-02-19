CREATE TABLE [dbo].[CanalOperacion] (
    [CanalOperacionId] INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]      VARCHAR (50) NOT NULL,
    [Inhabilitado]     BIT          NULL,
    CONSTRAINT [PK_CanalOperacion] PRIMARY KEY CLUSTERED ([CanalOperacionId] ASC)
);

