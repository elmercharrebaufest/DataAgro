CREATE TABLE [dbo].[StoresDeUsuarios] (
    [EmpresaID] INT           NOT NULL,
    [Nombre]    VARCHAR (100) NOT NULL,
    [Store]     VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_StoresDeUsuarios] PRIMARY KEY CLUSTERED ([EmpresaID] ASC, [Nombre] ASC)
);

