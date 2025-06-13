CREATE TABLE [dbo].[HabilitacionJob]
(
	[Id] INT NOT NULL PRIMARY KEY   IDENTITY (1, 1) ,
    [Nombre]      NVARCHAR (200) NOT NULL,
	[Habilitado] BIT NOT NULL
)

