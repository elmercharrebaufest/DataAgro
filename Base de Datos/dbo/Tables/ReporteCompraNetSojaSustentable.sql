CREATE TABLE [dbo].[ReporteCompraNetSojaSustentable] (
 [Id]    INT           IDENTITY (1, 1) NOT NULL,    
    [APrecio]         NUMERIC(18, 2) NOT NULL,
    [AFijar]         NUMERIC(18, 2) NOT NULL,
	    [Total] NUMERIC(18, 2) NOT NULL, 
    CONSTRAINT [PK_ReporteCompraNetSojaSustentable] PRIMARY KEY CLUSTERED ([Id] ASC)
);

