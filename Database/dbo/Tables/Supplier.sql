CREATE TABLE [dbo].[Supplier] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [SupplierName] VARCHAR (100) NOT NULL,
    [CreatedOn]    DATETIME      NULL,
    [CreatedBy]    VARCHAR (100) NULL,
    [UpdatedOn]    DATETIME      NULL,
    [UpdatedBy]    VARCHAR (100) NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([ID] ASC)
);





