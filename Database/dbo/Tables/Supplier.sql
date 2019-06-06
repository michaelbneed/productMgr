CREATE TABLE [dbo].[Supplier] (
    [SupplierID]    INT           IDENTITY (1, 1) NOT NULL,
    [SupplierName]  VARCHAR (100) NULL,
    [SupplierEmail] VARCHAR (200) NULL,
    [CreatedOn]     DATETIME      NULL,
    [CreatedBy]     VARCHAR (100) NULL,
    [UpdatedOn]     DATETIME      NULL,
    [UpdatedBy]     VARCHAR (100) NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([SupplierID] ASC)
);

