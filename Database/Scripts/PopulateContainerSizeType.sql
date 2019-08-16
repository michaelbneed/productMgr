PRINT 'Populating ContainerSizeType';
 SET IDENTITY_INSERT dbo.[ContainerSizeType] ON;
 GO
 ;WITH [source] AS (
	SELECT [ID], [Name]
	FROM ( VALUES
		( 1, 'Ounce (Oz)'),
		( 2, 'Milliliter (ML)'),
		( 3, 'Liter (L)' ),
		( 4, 'Pint (Pt)')
	) AS s([ID], [Name])
)
MERGE INTO dbo.[ContainerSizeType] [target]
USING [source] ON [target].[ID] = [source].[ID]
WHEN MATCHED AND target.[ContainerSizeTypeName] != source.[Name]
	THEN UPDATE SET [ContainerSizeTypeName] = source.[Name]
WHEN NOT MATCHED by target
	THEN INSERT ([ID],[ContainerSizeTypeName]) VALUES(source.[ID],source.[Name])
WHEN NOT MATCHED by source
	THEN DELETE;
 GO
 SET IDENTITY_INSERT dbo.[ContainerSizeType] OFF;
 DBCC CHECKIDENT('dbo.ContainerSizeType', RESEED) WITH NO_INFOMSGS;
 GO