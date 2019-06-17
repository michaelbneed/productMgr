﻿CREATE TABLE [dbo].[ProductPackageType] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [Quantity]  DECIMAL (18)  NULL,
    [Unit]      VARCHAR (50)  NULL,
    [ProductID] INT           NULL,
    [CreatedOn] DATETIME      NULL,
    [CreatedBy] VARCHAR (100) NULL,
    [UpdatedOn] DATETIME      NULL,
    [UpdatedBy] VARCHAR (100) NULL,
    CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ProductPackageType_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID])
);
