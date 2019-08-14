CREATE TABLE [dbo].[ContainerType] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [ContainerTypeName] NVARCHAR (100) NULL,
    CONSTRAINT [PK_ContainerType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

