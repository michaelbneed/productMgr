PRINT 'Populating ContainerType';
 SET IDENTITY_INSERT dbo.[ContainerType] ON;
 GO
 ;WITH [source] AS (
	SELECT [ID], [Name]
	FROM ( VALUES
		( 1, 'Glass Bottle'),
		( 2, 'Plastic Bottle'),
		( 3, 'Can' ),
		( 4, 'Case'),
		( 5, 'Carton'),
		( 6, 'Keg'),
		( 7, 'Each')
	) AS s([ID], [Name])
)
MERGE INTO dbo.[ContainerType] [target]
USING [source] ON [target].[ID] = [source].[ID]
WHEN MATCHED AND target.[ContainerTypeName] != source.[Name]
	THEN UPDATE SET [ContainerTypeName] = source.[Name]
WHEN NOT MATCHED by target
	THEN INSERT ([ID],[ContainerTypeName]) VALUES(source.[ID],source.[Name])
WHEN NOT MATCHED by source
	THEN DELETE;
 GO
 SET IDENTITY_INSERT dbo.[ContainerType] OFF;
 DBCC CHECKIDENT('dbo.ContainerType', RESEED) WITH NO_INFOMSGS;
 GO