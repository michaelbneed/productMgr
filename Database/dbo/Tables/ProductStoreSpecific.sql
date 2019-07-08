CREATE TABLE [dbo].[ProductStoreSpecific] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [ProductID]     INT           NULL,
    [PackageTypeID] INT           NULL,
    [StoreName]     VARCHAR (100) NOT NULL,
    [StorePrice]    MONEY         NULL,
    [StoreCost]     MONEY         NULL,
    [CreatedOn]     DATETIME      NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [UpdatedOn]     DATETIME      NULL,
    [UpdatedBy]     VARCHAR (100) NULL,
    CONSTRAINT [PK_ProductStoreSpecific] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ProductStoreSpecific_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID]),
    CONSTRAINT [FK_ProductStoreSpecific_ProductPackageType] FOREIGN KEY ([PackageTypeID]) REFERENCES [dbo].[ProductPackageType] ([ID])
);





