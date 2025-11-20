-- NOTE only aggreagates for the current AY unless is reset, in which case will redo 5 years

CREATE PROCEDURE [dbo].[GenerateProviderAttributesSummary]
(
    @minimumNumberOfReviews int = 5,
    @calcdate datetime = NULL,
    @reset int = 0  -- set to 1 to do a full reset
)
AS

BEGIN
    IF @calcdate IS NULL
    -- Default is now, but can be overriden for testing / back dating
        SET @calcdate = GETUTCDATE();
        
    -- Set limit to 5 years
    DECLARE 
    @limit5AY varchar(6) = 'AY'+RIGHT(YEAR(DATEADD(month,-55,@calcdate)),2)+RIGHT(YEAR(DATEADD(month,-43,@calcdate)),2),
    @limit1AY varchar(6) = 'AY'+RIGHT(YEAR(DATEADD(month,-7,@calcdate)),2)+RIGHT(YEAR(DATEADD(month,5,@calcdate)),2),
    @timelimit varchar(6),
    @startdate date = CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,-7,@calcdate)))+'-Aug-01'),
    @enddate date =   CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,5,@calcdate)))+'-Aug-01');
    
    SET @timelimit = @limit1AY;
    IF @reset = 1
    -- reset all 5 AYs
    BEGIN
        SET @startdate = DATEADD(year,-4,@startdate);
        SET @timelimit = @limit5AY;
    END;

    WITH LatestResults 
    AS (
        SELECT ar1.ApprenticeFeedbackTargetId, pa1.AttributeId, pa1.AttributeValue, ar1.Ukprn, TimePeriod
        FROM (
          -- get latest feedback for each feedback target
            SELECT * FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY TimePeriod,ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, *
                FROM (
                    SELECT ar1.*
                          ,'AY'+RIGHT(YEAR(DATEADD(month,-7,DateTimeCompleted)),2)+RIGHT(YEAR(DATEADD(month,5,DateTimeCompleted)),2) TimePeriod
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
            SELECT AttributeId, AttributeValue, Ukprn, TimePeriod, COUNT(*) OVER (PARTITION BY TimePeriod, Ukprn, AttributeId) ReviewCount
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
    WHEN NOT MATCHED BY SOURCE AND TimePeriod BETWEEN @limit1AY AND @timelimit THEN
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
    WHEN NOT MATCHED BY SOURCE AND TimePeriod = 'All' THEN
        DELETE;
    
  
END
GO
