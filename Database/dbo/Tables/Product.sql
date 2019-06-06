CREATE TABLE [dbo].[Product] (
    [ProductID]          INT            IDENTITY (1, 1) NOT NULL,
    [ProductName]        VARCHAR (100)  NOT NULL,
    [ProductDescription] NVARCHAR (MAX) NULL,
    [UPCCode]            VARCHAR (50)   NULL,
    [ProductLocation]    VARCHAR (100)  NULL,
    [ProductCost]        DECIMAL (18)   NULL,
    [ProductPrice]       DECIMAL (18)   NULL,
    [CreatedOn]          DATETIME       NULL,
    [CreatedBy]          VARCHAR (100)  NULL,
    [UpdatedOn]          DATETIME       NULL,
    [UpdatedBy]          VARCHAR (100)  NULL,
    CONSTRAINT [PK_Prooduct] PRIMARY KEY CLUSTERED ([ProductID] ASC)
);

