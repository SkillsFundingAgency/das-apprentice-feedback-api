-- backup the ApprenticeExitSurvey data once
		
IF NOT EXISTS 
(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_SCHEMA = 'dbo' 
	AND TABLE_NAME LIKE 'ApprenticeExitSurvey_Backup'
)
BEGIN
-- table to backup the ApprenticeExitSurvey data 
	CREATE TABLE [dbo].[ApprenticeExitSurvey_Backup]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER NOT NULL, 
		[StandardUId] NVARCHAR(12),
		[DateTimeCompleted] DateTime2 NOT NULL,
		[DidNotCompleteApprenticeship] BIT NULL DEFAULT 0,
		[IncompletionReason] NVARCHAR(100),
		[IncompletionFactor_Caring] BIT NULL DEFAULT 0,
		[IncompletionFactor_Family] BIT NULL DEFAULT 0,
		[IncompletionFactor_Financial] BIT NULL DEFAULT 0,
		[IncompletionFactor_Mental] BIT NULL DEFAULT 0,
		[IncompletionFactor_Physical] BIT NULL DEFAULT 0,
		[IncompletionFactor_None] BIT NULL DEFAULT 0,
		[RemainedReason] NVARCHAR(100),
		[ReasonForIncorrect] NVARCHAR(100),
		[AllowContact] BIT NULL DEFAULT 0
	)
	
END
GO

-- Take a backup 
BEGIN
	DECLARE @oldCount int = 0, @newCount int = 0, @Sql NVARCHAR(500), @ParmDefinition  NVARCHAR(50);
	
	IF EXISTS 
	(
		SELECT * 
		FROM INFORMATION_SCHEMA.TABLES 
		WHERE TABLE_SCHEMA = 'dbo' 
		AND TABLE_NAME LIKE 'ApprenticeExitSurvey_Backup'
	) 
	AND EXISTS
	(
		SELECT * 
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_SCHEMA = 'dbo' 
		AND TABLE_NAME = 'ApprenticeExitSurvey'
		AND COLUMN_NAME = 'IncompletionReason'
	)
	BEGIN
		SET @Sql = N'INSERT INTO [dbo].[ApprenticeExitSurvey_Backup] SELECT * FROM [dbo].[ApprenticeExitSurvey]'+
					'WHERE [Id] NOT IN (SELECT [Id] FROM [dbo].[ApprenticeExitSurvey_Backup])';
		EXEC sp_executesql @Sql;
				
		SELECT @oldCount = COUNT(*) FROM [dbo].[ApprenticeExitSurvey];
		
		SET @ParmDefinition = N'@result Int OUTPUT';
		SET @Sql = 'SELECT @result = COUNT(*) FROM [dbo].[ApprenticeExitSurvey_Backup]';
		EXEC sp_executesql @Sql, @ParmDefinition, @result=@newCount OUTPUT;
		
		-- empty the columns we no longer need - this will allow DACPAC to work
		IF @oldCount = @newCount AND @oldCount > 0
		BEGIN
			PRINT 'Backup copied ' + CONVERT(varchar,@newCount) + ' rows to [dbo].[ApprenticeExitSurvey_Backup]'
			PRINT 'Empty [dbo].[ApprenticeExitSurvey]'
			-- empty the table - this will allow DACPAC to work
			TRUNCATE TABLE [dbo].[ApprenticeExitSurvey];

		END
	END
END
GO



