CREATE TABLE [dbo].[Request] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [RequestDescription] NVARCHAR (MAX) NOT NULL,
    [RequestTypeID]      INT            NULL,
    [StatusTypeID]       INT            NULL,
    [UserID]             VARCHAR (100)  NULL,
    [ProductID]          INT            NULL,
    [SupplierID]         INT            NULL,
    [CreatedOn]          DATETIME       NULL,
    [CreatedBy]          VARCHAR (100)  NULL,
    [UpdatedOn]          DATETIME       NULL,
    [UpdatedBy]          VARCHAR (100)  NULL,
    CONSTRAINT [PK_ProductRequest] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Request_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID]),
    CONSTRAINT [FK_Request_RequestTypes] FOREIGN KEY ([RequestTypeID]) REFERENCES [dbo].[RequestType] ([ID]),
    CONSTRAINT [FK_Request_StatusTypes] FOREIGN KEY ([StatusTypeID]) REFERENCES [dbo].[StatusType] ([ID]),
    CONSTRAINT [FK_Request_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([ID])
);







