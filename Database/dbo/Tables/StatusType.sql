CREATE TABLE [dbo].[StatusType] (
    [StatusTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [StatusType]   VARCHAR (100) NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([StatusTypeID] ASC)
);

