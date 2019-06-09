CREATE TABLE [dbo].[Package] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [PackageName]  VARCHAR (100) NOT NULL,
    [PackagePrice] DECIMAL (18)  NULL,
    [PackageSize]  DECIMAL (18)  NULL,
    [ProductID]    INT           NULL,
    [CreatedOn]    DATETIME      NULL,
    [CreatedBy]    VARCHAR (100) NULL,
    [UpdatedOn]    DATETIME      NULL,
    [UpdatedBy]    VARCHAR (100) NULL,
    CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Package_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID])
);



