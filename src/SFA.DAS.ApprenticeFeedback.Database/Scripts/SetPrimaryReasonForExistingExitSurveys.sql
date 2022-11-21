-- For existing apprentice exit surveys
-- Migrate the backed-up data
-- and set the primary reason using the data from the backup table
BEGIN
	-- check if have a backup table, and new table structure is in place
	DECLARE @rowCount int = 0, @backupCount int = 0;
	
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
		AND table_name = 'ApprenticeExitSurvey'
		AND column_name = 'PrimaryReason'
	)
	BEGIN

		SELECT @backupCount = COUNT(*) FROM [dbo].[ApprenticeExitSurvey_Backup]
		
		SELECT @rowCount = COUNT(*) FROM [dbo].[ApprenticeExitSurvey]
		IF @rowCount = 0 AND @backupCount > 0
		-- backup table has some contents and Exit Survey table has been cleared
		BEGIN
		
			INSERT INTO [dbo].[ApprenticeExitSurvey] 
			([Id],[ApprenticeFeedbackTargetId],[StandardUId],[DateTimeCompleted],[DidNotCompleteApprenticeship],[AllowContact]) 
			SELECT [Id],[ApprenticeFeedbackTargetId],[StandardUId],[DateTimeCompleted],[DidNotCompleteApprenticeship],[AllowContact] 
			FROM [dbo].[ApprenticeExitSurvey_Backup];
			
			-- can now also save the attributes and set the add the primary reason
			 -- Incompletion Reason
			INSERT INTO [dbo].[ExitSurveyAttribute]
			SELECT axb.[Id] [ApprenticeExitSurveyId],  att.[AttributeId], 1 [AttributeValue] 
			FROM [dbo].[ApprenticeExitSurvey_Backup] axb
			JOIN [dbo].[ApprenticeExitSurvey] axs on axs.[Id] = axb.[id]
			LEFT JOIN (SELECT *
					 FROM [dbo].[Attribute]
					 WHERE AttributeType = 'ExitSurvey_v1' AND Category =  'Incompletion Reason' ) att on att.AttributeName = axb.IncompletionReason
			WHERE att.[AttributeId] IS NOT NULL
			AND axs.[PrimaryReason] = 0

			-- map PrimaryReason back from ExitSurveyAttribute to ApprenticeExitSurvey
			MERGE INTO [dbo].[ApprenticeExitSurvey] apx
			USING 
			(SELECT ex1.[ApprenticeExitSurveyId], ex1.[AttributeId] FROM [dbo].[ExitSurveyAttribute] ex1 
			   JOIN (SELECT * FROM [dbo].[Attribute] 
					 WHERE AttributeType = 'ExitSurvey_v1' 
					 AND Category = 'Incompletion Reason' ) exa ON ex1.[AttributeId] = exa.[AttributeId]
			) exa
			ON apx.[Id] = exa.[ApprenticeExitSurveyId]
			WHEN MATCHED THEN UPDATE SET apx.[PrimaryReason] = exa.[AttributeId];

			-- Incompletion factors
			INSERT INTO [dbo].[ExitSurveyAttribute]
			SELECT axb.[Id] [ApprenticeExitSurveyId], att.[AttributeId], 1 [AttributeValue] 
			FROM [dbo].[ApprenticeExitSurvey_Backup] axb
			JOIN (
			SELECT [Id], CASE WHEN [IncompletionFactor_Caring]= 1 THEN 'Caring responsibilities' ELSE NULL END Factor_Caring FROM [dbo].[ApprenticeExitSurvey_Backup]
			UNION 
			SELECT [Id], CASE WHEN [IncompletionFactor_Family] = 1 THEN 'Family or relationship issues' ELSE NULL END Factor_Caring FROM [dbo].[ApprenticeExitSurvey_Backup]
			UNION 
			SELECT [Id], CASE WHEN [IncompletionFactor_Financial] = 1 THEN 'Financial issues' ELSE NULL END Factor_Caring FROM [dbo].[ApprenticeExitSurvey_Backup]
			UNION 
			SELECT [Id], CASE WHEN [IncompletionFactor_Mental] = 1 THEN 'Mental health issues' ELSE NULL END Factor_Caring FROM [dbo].[ApprenticeExitSurvey_Backup]
			UNION 
			SELECT [Id], CASE WHEN [IncompletionFactor_Physical] = 1 THEN 'Physical health issues' ELSE NULL END Factor_Caring FROM [dbo].[ApprenticeExitSurvey_Backup]
			UNION 
			SELECT [Id], CASE WHEN [IncompletionFactor_None] = 1 THEN 'None of the above' ELSE NULL END Factor_Caring FROM [dbo].[ApprenticeExitSurvey_Backup]
			) ab1 ON axb.[Id] = ab1.[id] AND ab1.Factor_Caring IS NOT NULL
			LEFT JOIN (SELECT *
					 FROM [dbo].[Attribute]
					 WHERE AttributeType = 'ExitSurvey_v1' 
					   AND Category =  'Incompletion Factor' 
					   ) att on att.AttributeName = ab1.Factor_Caring
			WHERE att.[AttributeId] IS NOT NULL
			AND NOT EXISTS (SELECT NULL FROM [dbo].[ExitSurveyAttribute] WHERE [ApprenticeExitSurveyId] = axb.[Id] AND [AttributeId] = att.[AttributeId] )

			-- Remained Reason
			INSERT INTO [dbo].[ExitSurveyAttribute]
			SELECT axb.[Id] [ApprenticeExitSurveyId],  att.[AttributeId], 1 [AttributeValue] 
			FROM [dbo].[ApprenticeExitSurvey_Backup] axb
			JOIN [dbo].[ApprenticeExitSurvey] axs on axs.[Id] = axb.[id]
			LEFT JOIN (SELECT *
					 FROM [dbo].[Attribute]
					 WHERE AttributeType = 'ExitSurvey_v1' AND Category =  'Reason to remain' ) att on att.AttributeName = axb.[RemainedReason]
			WHERE att.[AttributeId] IS NOT NULL
			AND NOT EXISTS (SELECT NULL FROM [dbo].[ExitSurveyAttribute] WHERE [ApprenticeExitSurveyId] = axb.[Id] AND [AttributeId] = att.[AttributeId] )

			-- Reason for Incorrect
			INSERT INTO [dbo].[ExitSurveyAttribute]
			SELECT axb.[Id] [ApprenticeExitSurveyId],  att.[AttributeId], 1 [AttributeValue] 
			FROM [dbo].[ApprenticeExitSurvey_Backup] axb
			JOIN [dbo].[ApprenticeExitSurvey] axs on axs.[Id] = axb.[id]
			LEFT JOIN (SELECT *
					 FROM [dbo].[Attribute]
					 WHERE AttributeType = 'ExitSurvey_v1' AND Category =  'Your Apprenticeship' ) att on att.AttributeName = axb.[ReasonForIncorrect]
			WHERE att.[AttributeId] IS NOT NULL
			AND NOT EXISTS (SELECT NULL FROM [dbo].[ExitSurveyAttribute] WHERE [ApprenticeExitSurveyId] = axb.[Id] AND [AttributeId] = att.[AttributeId] )


		END
	END
END
GO





