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
            -- that are not for apprenticeships which are no longer relevant
            AND NOT EXISTS
            (
               SELECT NULL
               FROM [dbo].[IdentifyOldApprenticeships](@CurrentUtcDate) oa
               WHERE oa.Id = aft1.[Id]
            )
        ) aft
        CROSS JOIN [dbo].[EngagementEmails] ep1 
        WHERE ep1.[ProgrammeType] = aft.[DurationType]
        AND ( ep1.[MonthsBeforeEnd] IS NOT NULL -- this always includes the Start/Welcome email and PreEPA 
              OR DATEADD(month,[MonthsFromStart],[StartDate]) BETWEEN DATEADD(month,-1,DATEADD(day,1,EOMONTH(@CurrentUtcDate))) AND EOMONTH([EndDate])
            )
        )

        -- add new Engagement Email transactions for apprenticeships that do not yet have them
        INSERT INTO [dbo].[FeedbackTransaction] ([ApprenticeFeedbackTargetId] ,[CreatedOn] ,[SendAfter], [TemplateName])
        SELECT [ApprenticeFeedbackTargetId], COALESCE(@specifiedUtcDate, GETUTCDATE()) [CreatedOn], SendAfter, [TemplateName] 
        FROM EmailSchedule
        ORDER BY [ApprenticeFeedbackTargetId], SendAfter, seqn;

        SET @Count = @Count + @@ROWCOUNT;
        
        -- remove existing Engagement Email transactions that are for apprenticeships which are no longer relevant
        DELETE fbt FROM [dbo].[FeedbackTransaction] fbt
        INNER JOIN [dbo].[IdentifyOldApprenticeships](@CurrentUtcDate) oa on oa.[Id] = fbt.[ApprenticeFeedbackTargetId]
        WHERE 1=1
            AND fbt.[SendAfter] >= @CurrentUtcDate
            AND fbt.[SentDate] IS NULL
            AND fbt.[TemplateName] IS NOT NULL
            AND fbt.[TemplateName] IN (SELECT [TemplateName] FROM [dbo].[EngagementEmails] ) 

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
