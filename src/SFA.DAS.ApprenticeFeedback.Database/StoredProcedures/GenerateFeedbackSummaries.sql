-- Apprentice Feedback
-- NOTE only aggregates for the current AY unless is reset, in which case will redo 5 years

CREATE PROCEDURE [dbo].[GenerateFeedbackSummaries]
(
    @minimumNumberOfReviews int = 5,
    @reset int = 0,            -- set to 1 to do a full reset of the past 5 years
    @rundate datetime = NULL   -- set this for TESTING only
 )
AS
BEGIN

-------------------------------------------------------------------------------
-- Set the date ranges to use
-------------------------------------------------------------------------------
    DECLARE 
    @attributes int = 12,   -- the number of attributes asked per feeback questionnaire
    @calcdate datetime,     -- date of calculation, normally NOW but can be back-dated 
    @limit5AY varchar(6),   -- earliest AY for the 5 Year "All" calculations
    @startAY varchar(6),    -- AY to start summarisation, normally current AY but can be earlier at AY boundary and for reset  
    @endAY varchar(6),      -- AY to end summaristions, normally current AY but can differ for back-dated  
    @startdate date,        -- Start date, e.g 2025-Aug-01 for the start of sumamrisation
    @enddate date,          -- next day beyond the period of sumamrisation, e.g. 2026-Aug-01
    @lastupdate datetime;   -- date for previous summarisation update, if any
    
    -- Default is now, but can be overridden for testing / back dating
    SET @calcdate = ISNULL(@rundate,GETUTCDATE());
    SET @endAY = [dbo].[ConvertAY](@calcdate);

    -- Set limit to 5 years from calc date for 'All'
    SET @limit5AY = [dbo].[ConvertAY](DATEADD(YEAR,-4,@calcdate));

    IF @reset = 1
    -- reset all 5 AYs
        SET @startdate = CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,-55,@calcdate)))+'-Aug-01');
    ELSE
    BEGIN
    -- complication if calcdate is in a new AY, but last update was in previous AY
    -- in which case need to process current and previous AYs
        SELECT @lastupdate = MAX(UpdatedOn) FROM [dbo].[ProviderAttributeSummary];
        IF @lastupdate IS NOT NULL
            SET @startdate = CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,-7,@lastupdate)))+'-Aug-01');
        ELSE
            SET @startdate = CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,-7,@calcdate)))+'-Aug-01');
    END;
    SET @enddate = CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,5,@calcdate)))+'-Aug-01');
        
    SET @startAY = [dbo].[ConvertAY](@startdate);

-------------------------------------------------------------------------------
-- Process ProviderAttributeSummary
-------------------------------------------------------------------------------

    WITH LatestResults 
    AS (
        SELECT ar1.ApprenticeFeedbackTargetId, pa1.AttributeId, pa1.AttributeValue, ar1.Ukprn, ar1.TimePeriod
        FROM (
          -- get latest feedback for each feedback target
            SELECT Id, ApprenticeFeedbackTargetId, Ukprn, TimePeriod   
            FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY TimePeriod,ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, Id, ApprenticeFeedbackTargetId, Ukprn, TimePeriod 
                FROM (
                    SELECT ar1.Id, ApprenticeFeedbackTargetId, Ukprn, DateTimeCompleted
                          ,[dbo].[ConvertAY](DateTimeCompleted) TimePeriod
                    FROM [dbo].[ApprenticeFeedbackResult] ar1
                    WHERE (@reset =1 OR (DateTimeCompleted >= @startdate AND DateTimeCompleted < @enddate))
                    AND ar1.Ukprn IS NOT NULL
                ) afr
            ) ab1 
            WHERE seq = 1 
        ) ar1
        JOIN [dbo].[ProviderAttribute] pa1 on pa1.ApprenticeFeedbackResultId = ar1.Id
    )
    -- Get the ratings for required AY results for each UKPRN
    MERGE INTO [dbo].[ProviderAttributeSummary] pas 
    USING (  

        -- Year-on-Year Results
        SELECT TimePeriod, Ukprn, AttributeId
              ,SUM(AttributeValue) Agree 
              ,SUM(CASE WHEN AttributeValue = 1 THEN 0 ELSE 1 END) Disagree
        FROM (
            SELECT AttributeId, AttributeValue, Ukprn, TimePeriod, COUNT(*) OVER (PARTITION BY TimePeriod, Ukprn)/@attributes ReviewCount
            FROM LatestResults
        ) ab1
        WHERE ReviewCount >= @minimumNumberOfReviews
        GROUP BY TimePeriod, Ukprn, AttributeId

     ) upd
    ON pas.Ukprn = upd.Ukprn AND pas.AttributeId = upd.AttributeId AND pas.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN 
        UPDATE SET pas.Agree = upd.Agree, 
                   pas.Disagree = upd.Disagree,
                   pas.UpdatedOn = @calcdate
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, AttributeId, Agree, Disagree, UpdatedOn, TimePeriod) 
        VALUES (upd.Ukprn, upd.AttributeId, upd.Agree, upd.Disagree, @calcdate, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE AND pas.TimePeriod BETWEEN @startAY AND @endAY THEN
        DELETE;

    -- Get the ratings for all eligible 5 Year results for each UKPRN
    MERGE INTO [dbo].[ProviderAttributeSummary] pas 
    USING (  
        -- All, adding each year's results
        SELECT 'All' TimePeriod, Ukprn, AttributeId
               ,SUM(Agree) Agree 
               ,SUM(Disagree) Disagree 
        FROM [dbo].[ProviderAttributeSummary] 
        WHERE TimePeriod >= @limit5AY  -- will ignore 'All'
        GROUP BY Ukprn, AttributeId
     ) upd
    ON pas.Ukprn = upd.Ukprn AND pas.AttributeId = upd.AttributeId AND pas.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN 
        UPDATE SET pas.Agree = upd.Agree, 
                   pas.Disagree = upd.Disagree,
                   pas.UpdatedOn = @calcdate
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, AttributeId, Agree, Disagree, UpdatedOn, TimePeriod) 
        VALUES (upd.Ukprn, upd.AttributeId, upd.Agree, upd.Disagree, @calcdate, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE AND pas.TimePeriod = 'All' THEN
        DELETE;
    

-------------------------------------------------------------------------------
-- Process ProviderRatingSummary
-------------------------------------------------------------------------------

    WITH LatestRatings 
    AS (
        SELECT ar1.ApprenticeFeedbackTargetId, ar1.ProviderRating, ar1.Ukprn, ar1.TimePeriod
        FROM (
          -- get latest feedback for each feedback target
            SELECT ApprenticeFeedbackTargetId, Ukprn, ProviderRating, TimePeriod 
            FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY TimePeriod,ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, ApprenticeFeedbackTargetId, Ukprn, ProviderRating, TimePeriod
                FROM (
                    SELECT ApprenticeFeedbackTargetId, Ukprn, ProviderRating, DateTimeCompleted
                          ,[dbo].[ConvertAY](DateTimeCompleted) TimePeriod
                    FROM [dbo].[ApprenticeFeedbackResult] ar1
                    WHERE (@reset =1 OR (DateTimeCompleted >= @startdate AND DateTimeCompleted < @enddate))
                    AND ar1.Ukprn IS NOT NULL
                ) afr
            ) ab1 
            WHERE seq = 1 
        ) ar1
    )
    -- Get the ratings for required AY results for each UKPRN (not 'All')
    MERGE INTO [dbo].[ProviderRatingSummary] prs 
    USING (  

        -- Year-on-Year Results
        SELECT TimePeriod, Ukprn
              ,ProviderRating Rating 
              ,COUNT(*) RatingCount
        FROM (
            SELECT Ukprn, TimePeriod, ProviderRating
                  ,COUNT(*) OVER (PARTITION BY TimePeriod, Ukprn) ReviewCount
            FROM LatestRatings
        ) ab1
        WHERE ReviewCount >= @minimumNumberOfReviews
        GROUP BY TimePeriod, Ukprn, ProviderRating
     ) upd
    ON prs.Ukprn = upd.Ukprn AND prs.Rating = upd.Rating AND prs.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN 
        UPDATE SET prs.RatingCount = upd.RatingCount, 
                   prs.UpdatedOn = @calcdate
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, Rating, RatingCount, UpdatedOn, TimePeriod)
        VALUES (upd.Ukprn, upd.Rating, upd.RatingCount, @calcdate, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE AND prs.TimePeriod BETWEEN @startAY AND @endAY THEN
        DELETE;

-------------------------------------------------------------------------------
-- Process ProviderStarsSummary
-------------------------------------------------------------------------------

    -- Get the Stars for all eligible 5 Year results for each UKPRN
    WITH av1 
    AS(
        SELECT TimePeriod, Ukprn, ReviewCount,
            CASE
            WHEN AvgRating >= 3.5 THEN 4
            WHEN AvgRating >= 2.5 THEN 3
            WHEN AvgRating >= 1.5 THEN 2
            ELSE 1 END Stars
        FROM (
            SELECT TimePeriod, Ukprn, SUM(RatingCount) ReviewCount,
                ROUND(CAST(SUM((CASE [Rating] WHEN 'VeryPoor' THEN 1 WHEN 'Poor' THEN 2 WHEN 'Good' THEN 3 WHEN 'Excellent' THEN 4 ELSE 1 END) * RatingCount) AS FLOAT) / CAST(SUM(RatingCount) AS FLOAT), 1) AvgRating
            FROM [dbo].[ProviderRatingSummary]
            GROUP BY TimePeriod, Ukprn
        ) ab2
    )

    MERGE INTO [dbo].[ProviderStarsSummary] pss
    USING (
        SELECT TimePeriod, Ukprn, ReviewCount, Stars
        FROM av1
        UNION ALL
        -- All, adding each year's results
        SELECT 'All' TimePeriod, Ukprn
               ,SUM(ReviewCount) ReviewCount 
               ,ROUND(AVG(CAST(Stars AS FLOAT)), 0) Stars
        FROM av1 
        WHERE TimePeriod >= @limit5AY  -- will ignore 'All'
        GROUP BY Ukprn        
    ) upd
    ON pss.Ukprn = upd.Ukprn AND pss.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN
        UPDATE SET pss.ReviewCount = upd.ReviewCount,
                   pss.Stars = upd.Stars
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Ukprn, ReviewCount, Stars, TimePeriod)
        VALUES (upd.Ukprn, upd.ReviewCount, upd.Stars, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;  
          
-------------------------------------------------------------------------------
-- Done
-------------------------------------------------------------------------------

END
GO
