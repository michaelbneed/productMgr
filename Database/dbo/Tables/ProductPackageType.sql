﻿CREATE TABLE [dbo].[ProductPackageType] (
    [ID]                      INT           IDENTITY (1, 1) NOT NULL,
    [Quantity]                DECIMAL (18)  NULL,
    [Unit]                    VARCHAR (50)  NULL,
    [AlternateProductName]    VARCHAR (100) NULL,
    [AlternateProductUPCCode] VARCHAR (50)  NULL,
    [SupplierData]            VARCHAR (50)  NULL,
    [SupplierID]              INT           NULL,
    [AlternateProductPrice]   DECIMAL (18)  NULL,
    [AlternateProductCost]    NCHAR (10)    NULL,
    [ProductID]               INT           NULL,
    [CreatedOn]               DATETIME      NULL,
    [CreatedBy]               VARCHAR (100) NULL,
    [UpdatedOn]               DATETIME      NULL,
    [UpdatedBy]               VARCHAR (100) NULL,
    CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ProductPackageType_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID]),
    CONSTRAINT [FK_ProductPackageType_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([ID])
);





