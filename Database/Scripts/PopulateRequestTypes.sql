PRINT 'Populating RequestType';
 SET IDENTITY_INSERT dbo.[RequestType] ON;
 GO
 ;WITH [source] AS (
	SELECT [ID], [Name]
	FROM ( VALUES
		( 1, 'New Product'),
		( 2, 'New Vendor'),
		( 3, 'Product Price' ),
		( 4, 'Product Issue')
	) AS s([ID], [Name])
)
MERGE INTO dbo.[RequestType] [target]
USING [source] ON [target].[ID] = [source].[ID]
WHEN MATCHED AND target.[RequestTypeName] != source.[Name]
	THEN UPDATE SET [RequestTypeName] = source.[Name]
WHEN NOT MATCHED by target
	THEN INSERT ([ID],[RequestTypeName]) VALUES(source.[ID],source.[Name])
WHEN NOT MATCHED by source
	THEN DELETE;
 GO
 SET IDENTITY_INSERT dbo.[RequestType] OFF;
 DBCC CHECKIDENT('dbo.RequestType', RESEED) WITH NO_INFOMSGS;
 GO