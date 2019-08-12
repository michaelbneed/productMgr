PRINT 'Populating Category';
 SET IDENTITY_INSERT dbo.[Category] ON;
 GO
 ;WITH [source] AS (
	SELECT [ID], [Name]
	FROM ( VALUES
		( 1, 'Beer'),
		( 2, 'Liquor'),
		( 3, 'Wine' ),
		( 4, 'Tobacco' ),
		( 5, 'Food' ),
		( 6, 'Misc')
	) AS s([ID], [Name])
)
MERGE INTO dbo.[Category] [target]
USING [source] ON [target].[ID] = [source].[ID]
WHEN MATCHED AND target.[CategoryName] != source.[Name]
	THEN UPDATE SET [CategoryName] = source.[Name]
WHEN NOT MATCHED by target
	THEN INSERT ([ID],[CategoryName]) VALUES(source.[ID],source.[Name])
WHEN NOT MATCHED by source
	THEN DELETE;
 GO
 SET IDENTITY_INSERT dbo.[Category] OFF;
 DBCC CHECKIDENT('dbo.Category', RESEED) WITH NO_INFOMSGS;
 GO