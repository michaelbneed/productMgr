CREATE TABLE [dbo].[Product] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [ProductName]         VARCHAR (100)  NOT NULL,
    [ProductDescription]  NVARCHAR (MAX) NULL,
    [UPCCode]             VARCHAR (50)   NULL,
    [ProductLocation]     VARCHAR (100)  NULL,
    [ProductCost]         MONEY          NULL,
    [ProductPrice]        MONEY          NULL,
    [SuggestedPrice]      MONEY          NULL,
    [PackageSize]         VARCHAR (50)   NULL,
    [ContainerSizeTypeID] INT            NULL,
    [ContainerTypeID]     INT            NULL,
    [OrderWeek]           VARCHAR (5)    NULL,
    [CategoryID]          INT            NULL,
    [CreatedOn]           DATETIME       NULL,
    [CreatedBy]           VARCHAR (100)  NULL,
    [UpdatedOn]           DATETIME       NULL,
    [UpdatedBy]           VARCHAR (100)  NULL,
    [UnitsPerCase] INT NULL, 
    [SupplierData] VARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_Prooduct] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Product_Category] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([ID]),
    CONSTRAINT [FK_Product_ContainerSizeType] FOREIGN KEY ([ContainerSizeTypeID]) REFERENCES [dbo].[ContainerSizeType] ([ID]),
    CONSTRAINT [FK_Product_ContainerType] FOREIGN KEY ([ContainerTypeID]) REFERENCES [dbo].[ContainerType] ([ID])
);


GO
ALTER TABLE [dbo].[Product] NOCHECK CONSTRAINT [FK_Product_ContainerSizeType];


GO
ALTER TABLE [dbo].[Product] NOCHECK CONSTRAINT [FK_Product_ContainerType];




GO
ALTER TABLE [dbo].[Product] NOCHECK CONSTRAINT [FK_Product_ContainerSizeType];


GO
ALTER TABLE [dbo].[Product] NOCHECK CONSTRAINT [FK_Product_ContainerType];















