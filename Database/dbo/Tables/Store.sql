CREATE TABLE [dbo].[Store] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [StoreName]            VARCHAR (100) NOT NULL,
    [StoreSupervisorName]  VARCHAR (100) NULL,
    [StoreSupervisorEmail] VARCHAR (200) NULL,
    [CreatedOn]            DATETIME      NULL,
    [CreatedBy]            VARCHAR (100) NULL,
    [UpdatedOn]            DATETIME      NULL,
    [UpdatedBy]            VARCHAR (100) NULL,
    CONSTRAINT [PK_Store] PRIMARY KEY CLUSTERED ([ID] ASC)
);

