CREATE TABLE [dbo].[Request] (
    [RequestID]          INT            IDENTITY (1, 1) NOT NULL,
    [RequestDescription] NVARCHAR (MAX) NOT NULL,
    [RequestTypeID]      INT            NULL,
    [StatusTypeID]       INT            NULL,
    [UserID]             INT            NULL,
    [ProductID]          INT            NULL,
    [SupplierID]         INT            NULL,
    [CreatedOn]          DATETIME       NULL,
    [CreatedBy]          VARCHAR (100)  NULL,
    [UpdatedOn]          DATETIME       NULL,
    [UpdatedBy]          VARCHAR (100)  NULL,
    CONSTRAINT [PK_ProductRequest] PRIMARY KEY CLUSTERED ([RequestID] ASC),
    CONSTRAINT [FK_Request_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ProductID]),
    CONSTRAINT [FK_Request_RequestTypes] FOREIGN KEY ([RequestTypeID]) REFERENCES [dbo].[RequestType] ([RequestTypeID]),
    CONSTRAINT [FK_Request_StatusTypes] FOREIGN KEY ([StatusTypeID]) REFERENCES [dbo].[StatusType] ([StatusTypeID]),
    CONSTRAINT [FK_Request_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID])
);



