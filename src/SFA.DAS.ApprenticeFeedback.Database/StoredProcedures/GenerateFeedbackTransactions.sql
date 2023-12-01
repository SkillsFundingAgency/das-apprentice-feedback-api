CREATE PROCEDURE [dbo].[GenerateFeedbackTransactions]

    @sentDateAgeDays INT = 90,
    @specifiedUtcDate DATETIME = NULL

AS
BEGIN
SET NOCOUNT ON;

    DECLARE @Error_Code INT = 0,
            @Error_Message VARCHAR(MAX),
            @Error_Severity INT = 0;

    BEGIN TRANSACTION T1;

    DECLARE @Count INT;
    DECLARE @CurrentUtcDate DATETIME = COALESCE(@specifiedUtcDate, GETUTCDATE())

    BEGIN TRY;

        -- Add the Feedback Email requests
        INSERT INTO dbo.FeedbackTransaction (ApprenticeFeedbackTargetId, CreatedOn)
        SELECT aft.Id, COALESCE(@specifiedUtcDate, GETUTCDATE()) [CreatedOn]
        FROM [dbo].[ApprenticeFeedbackTarget] aft
        LEFT JOIN (
            SELECT DISTINCT ApprenticeFeedbackTargetId
            FROM [dbo].[FeedbackTransaction]
            WHERE 1=1
            AND ( ( [TemplateName] IS NULL
                 OR [TemplateName] NOT IN (SELECT [TemplateName] FROM [dbo].[EngagementEmails] ) ) )
            AND ( [SentDate] IS NULL OR ( [SentDate] IS NOT NULL AND [SentDate] >= DATEADD(day, 0 - @sentDateAgeDays, GETDATE()) ) )
        ) ft1  on ft1.ApprenticeFeedbackTargetId = aft.[Id]
        WHERE aft.[Status] = 2 -- "active"
        AND aft.[FeedbackEligibility] = 1 -- "allow"
        AND ft1.ApprenticeFeedbackTargetId IS NULL;

        SET @Count = @@ROWCOUNT;

        -- identify any apprenticeships which have redundant Email Engagement programmes 
        -- from Feedback transactions where Apprenticeship has been replaced, withdrawn or superseded
        -- or has past the ended date; and where the Apprentice has confirmed a more recent Apprenticeship
        SELECT 
            aft1.[Id]
        INTO 
            #OldApprenticeships
        FROM [dbo].[ApprenticeFeedbackTarget] aft1
        WHERE 
            aft1.[Status] = 3 -- was completed
            OR aft1.[Withdrawn] = 1 -- was withdrawn
            OR aft1.[EndDate] < DATEADD(month,-2,EOMONTH(@CurrentUtcDate)) -- is beyond planned or estimated end date
        UNION
            -- find superseded Apprenticeships
        SELECT 
            aft2.[Id]
        FROM 
        (
            SELECT 
                [Id],
                COUNT(*) OVER (PARTITION BY [ApprenticeId]) HowManyApprenticeships,
                ROW_NUMBER() OVER (PARTITION BY [ApprenticeId] ORDER BY [CreatedOn] DESC, [ApprenticeshipId] DESC) seqn
            FROM [dbo].[ApprenticeFeedbackTarget]
        ) aft2
        WHERE HowManyApprenticeships > 1
        AND seqn > 1;

        -- This data will be inserted in to FeedbackTransaction
        -- Add the Email Engagement programme preset data for New Starts and Active targets
        WITH EmailSchedule
        AS
        (
        -- active targets
        SELECT 
        CASE WHEN ep1.[MonthsBeforeEnd] IS NULL 
             THEN LEAST(DATEADD(month,[MonthsFromStart],[StartDate]),[EndDate])
             ELSE GREATEST(LEAST(CONVERT(date,@CurrentUtcDate),[EndDate]),[StartDate],DATEADD(month,0-[MonthsBeforeEnd],[EndDate])) END SendAfter,
        [TemplateName] , [StartDate], [EndDate], aft.[Id] ApprenticeFeedbackTargetId,
        ep1.[Id] seqn
        FROM 
        (
            -- all potential active and new start targets
            SELECT [Id], [StartDate], [EndDate]
            -- start date to be from the start of the previous month
            ,CASE WHEN [StartDate] > EOMONTH(DATEADD(month,-2,@CurrentUtcDate)) THEN 'start' ELSE 'active' END +
             CASE WHEN DATEDIFF(month,[StartDate],[EndDate]) <= 24 THEN 'short' ELSE 'long' END [DurationType]
            ,DATEDIFF(month,[StartDate],[EndDate]) PlannedDuration
            FROM [dbo].[ApprenticeFeedbackTarget] aft1
            WHERE 1=1
            AND [IsTransfer] = 0
            AND [Withdrawn] = 0
            AND [StartDate] IS NOT NULL
            AND [EndDate] IS NOT NULL
            AND [EndDate] >= DATEADD(month,-1,DATEADD(day,1,EOMONTH(@CurrentUtcDate))) -- End date after start of the current month
            AND [Status] != 3 -- i.e. not (yet) Complete
            -- that have not yet had Engagement Emails added
            AND NOT EXISTS 
            (
                SELECT NULL 
                FROM  [dbo].[FeedbackTransaction] ft1
                JOIN [dbo].[EngagementEmails] ep1 on ep1.[TemplateName] = ft1.[TemplateName]
                WHERE ft1.[ApprenticeFeedbackTargetId] = aft1.[Id]
            )
            AND NOT EXISTS
            (
               SELECT NULL
               FROM #OldApprenticeships oa
               WHERE oa.Id = aft1.[Id]
            )
        ) aft
        CROSS JOIN [dbo].[EngagementEmails] ep1 
        WHERE ep1.[ProgrammeType] = aft.[DurationType]
        AND ( ep1.[MonthsBeforeEnd] IS NOT NULL -- this always includes the Start/Welcome email and PreEPA 
              OR DATEADD(month,[MonthsFromStart],[StartDate]) BETWEEN DATEADD(month,-1,DATEADD(day,1,EOMONTH(@CurrentUtcDate))) AND EOMONTH([EndDate])
            )
        )

        -- add a new Email Engagement programme for any Apprenticeships that have recently started and do not yet have one setup
        INSERT INTO [dbo].[FeedbackTransaction] ([ApprenticeFeedbackTargetId] ,[CreatedOn] ,[SendAfter], [TemplateName])
        SELECT [ApprenticeFeedbackTargetId], COALESCE(@specifiedUtcDate, GETUTCDATE()) [CreatedOn], SendAfter, [TemplateName] 
        FROM EmailSchedule
        ORDER BY [ApprenticeFeedbackTargetId], SendAfter, seqn;

        SET @Count = @Count + @@ROWCOUNT;
        
        --Remove pre-planned Engagement Email transactions that are no longer relevant
        MERGE INTO [dbo].[FeedbackTransaction] fbt
        USING (
            SELECT ft1.[Id]
            FROM [dbo].[FeedbackTransaction] ft1
            JOIN #OldApprenticeships oa on oa.[Id] = ft1.[ApprenticeFeedbackTargetId]
            WHERE 1=1
            AND ft1.[SendAfter] >= @CurrentUtcDate
            AND ft1.[SentDate] IS NULL
            AND ft1.[TemplateName] IS NOT NULL
            AND ft1.[TemplateName] IN (SELECT [TemplateName] FROM [dbo].[EngagementEmails] ) 
        ) del
        ON (fbt.[Id] = del.[Id])
        WHEN MATCHED THEN
        DELETE;

    END TRY
    BEGIN CATCH;
        -- Some basic error handling
        ROLLBACK TRANSACTION T1;
        SELECT @Error_Code = ERROR_NUMBER(), @Error_Message = ERROR_MESSAGE(), @Error_Severity = ERROR_SEVERITY();
        raiserror (@Error_Message, @Error_Severity,@Error_Code);
    END CATCH;

     IF @Error_Code = 0 AND XACT_STATE() = 1 
        COMMIT TRANSACTION T1;
        
    SELECT @Count AS Count, @CurrentUtcDate AS CreatedOn;

END;

GO
