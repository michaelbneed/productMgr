﻿CREATE TABLE [dbo].[Note] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [NoteText]  NVARCHAR (MAX) NOT NULL,
    [RequestID] INT            NULL,
    [CreatedOn] DATETIME       NULL,
    [CreatedBy] VARCHAR (100)  NULL,
    [UpdatedOn] DATETIME       NULL,
    [UpdatedBy] VARCHAR (100)  NULL,
    CONSTRAINT [PK_Notes_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Notes_Request] FOREIGN KEY ([RequestID]) REFERENCES [dbo].[Request] ([ID])
);



