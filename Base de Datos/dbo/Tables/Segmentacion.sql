CREATE TABLE [dbo].[Segmentacion] (
    [SegmentacionId] INT          NOT NULL,
    [Descripcion]    VARCHAR (50) NOT NULL,
    [Grupo]          VARCHAR (50) NULL,
    CONSTRAINT [PK_Segmentacion] PRIMARY KEY CLUSTERED ([SegmentacionId] ASC)
);

