CREATE TABLE [dbo].[RequestType] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [RequestTypeName] NVARCHAR (100) NULL,
    CONSTRAINT [PK_RequestTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);





