CREATE TABLE [dbo].[ContainerSizeType] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [ContainerSizeTypeName] NVARCHAR (100) NULL,
    CONSTRAINT [PK_ContainerSizeType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

