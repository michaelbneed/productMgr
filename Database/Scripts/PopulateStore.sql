PRINT 'Populating Store';
 SET IDENTITY_INSERT dbo.[Store] ON;
 GO
 ;WITH [source] AS (
	SELECT [ID], [Name], [Manager], [Email]
	FROM ( VALUES
		( 1, 'TestStore1', 'Michael', 'mneed@adaptivesys.com'),
		( 2, 'TestStore2', 'David', 'dpacenka@adaptivesys.com'),
		( 3, 'TestStore3', 'Doug', 'dwhitlock@adaptivesys.com')
	) AS s([ID], [Name], [Manager], [Email])
)
MERGE INTO dbo.[Store] [target]
USING [source] ON [target].[ID] = [source].[ID]
WHEN MATCHED AND target.[StoreName] != source.[Name]
	THEN UPDATE SET [StoreName] = source.[Name], [StoreSupervisorName] = source.[Manager], [StoreSupervisorEmail] = source.[Email]
WHEN NOT MATCHED by target
	THEN INSERT ([ID],[StoreName],[StoreSupervisorName],[StoreSupervisorEmail]) 
	VALUES(source.[ID],source.[Name], source.[Manager], source.[Email])
WHEN NOT MATCHED by source
	THEN DELETE;
 GO
 SET IDENTITY_INSERT dbo.[Store] OFF;
 DBCC CHECKIDENT('dbo.Store', RESEED) WITH NO_INFOMSGS;
 GO