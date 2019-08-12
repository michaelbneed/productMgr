PRINT 'Populating StatusType';
 SET IDENTITY_INSERT dbo.[StatusType] ON;
 GO
 ;WITH [source] AS (
	SELECT [ID], [Name]
	FROM ( VALUES
		( 1, 'New Request'),
		( 2, 'Approved'),
		( 3, 'Denied' ),
		( 4, 'Complete')
	) AS s([ID], [Name])
)
MERGE INTO dbo.[StatusType] [target]
USING [source] ON [target].[ID] = [source].[ID]
WHEN MATCHED AND target.[StatusTypeName] != source.[Name]
	THEN UPDATE SET [StatusTypeName] = source.[Name]
WHEN NOT MATCHED by target
	THEN INSERT ([ID],[StatusTypeName]) VALUES(source.[ID],source.[Name])
WHEN NOT MATCHED by source
	THEN DELETE;
 GO
 SET IDENTITY_INSERT dbo.[StatusType] OFF;
 DBCC CHECKIDENT('dbo.StatusType', RESEED) WITH NO_INFOMSGS;
 GO