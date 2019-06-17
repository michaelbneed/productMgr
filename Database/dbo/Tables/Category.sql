CREATE TABLE [dbo].[Category] (
    [ID]           INT          IDENTITY (1, 1) NOT NULL,
    [CategoryName] VARCHAR (50) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([ID] ASC)
);

