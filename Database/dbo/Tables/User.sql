CREATE TABLE [dbo].[User] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [SupplierID]   INT           NULL,
    [FirstName]    VARCHAR (100) NOT NULL,
    [LastName]     VARCHAR (100) NOT NULL,
    [EmailAddress] VARCHAR (200) NOT NULL,
    [AuthID]       VARCHAR (100) NULL,
    [Enabled]      BIT           NULL,
    [CreatedOn]    DATETIME      NULL,
    [CreatedBy]    VARCHAR (100) NULL,
    [UpdatedOn]    DATETIME      NULL,
    [UpdatedBy]    VARCHAR (100) NULL,
    CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_User_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([ID])
);









