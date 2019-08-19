CREATE TABLE [dbo].[RequestLog] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [RequestID]             INT            NULL,
    [RequestDescription]    NVARCHAR (MAX) NULL,
    [RequestTypeID]         INT            NULL,
    [StatusTypeID]          INT            NULL,
    [UserID]                VARCHAR (100)  NULL,
    [ProductID]             INT            NULL,
    [SupplierID]            INT            NULL,
    [StoreID]               INT            NULL,
    [ChangeNote]            VARCHAR (100)  NULL,
    [OriginalCreatedOnDate] DATETIME       NULL,
    [OriginalCreatedByUser] VARCHAR (100)  NULL,
    [CreatedOn]             DATETIME       NULL,
    [CreatedBy]             VARCHAR (100)  NULL,
    CONSTRAINT [PK__RequestL__3214EC274D822615] PRIMARY KEY CLUSTERED ([ID] ASC)
);

