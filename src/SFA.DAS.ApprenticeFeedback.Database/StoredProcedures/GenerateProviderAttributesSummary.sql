CREATE PROCEDURE [dbo].[GenerateProviderAttributesSummary]
(
    @recentFeedbackMonths INT,
    @minimumNumberOfReviews INT
)
AS
BEGIN
    DECLARE @CurrentYear INT = YEAR(GETDATE());
	DECLARE @TimePeriods TABLE (TimePeriod VARCHAR(10));
    INSERT INTO @TimePeriods (TimePeriod) VALUES ('All');
    DECLARE @i INT = 0;
    WHILE @i <= 5
    BEGIN
        DECLARE @StartYear INT = @CurrentYear - @i - 1;
        DECLARE @EndYear INT = @CurrentYear - @i;
        DECLARE @TimePeriodTemp VARCHAR(10) = CONCAT('AY', RIGHT(CAST(@StartYear AS VARCHAR), 2), RIGHT(CAST(@EndYear AS VARCHAR), 2));
        INSERT INTO @TimePeriods (TimePeriod) VALUES (@TimePeriodTemp);
        SET @i = @i + 1;
    END
    DECLARE @TimePeriod VARCHAR(10);
    DECLARE @StartDate DATETIME, @EndDate DATETIME;
    DECLARE TimePeriodCursor CURSOR FOR SELECT TimePeriod FROM @TimePeriods;
    OPEN TimePeriodCursor;
    FETCH NEXT FROM TimePeriodCursor INTO @TimePeriod;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @TimePeriod = 'All'
        BEGIN
            SET @StartDate = DATEADD(MONTH,-@recentFeedbackMonths,GETUTCDATE())
            SET @EndDate = GETUTCDATE();
        END
        ELSE
        BEGIN
           SET  @StartYear  = CAST('20' + SUBSTRING(@TimePeriod, 3, 2) AS INT);
          SET  @EndYear  = CAST('20' + SUBSTRING(@TimePeriod, 5, 2) AS INT);
            SET @StartDate = DATEFROMPARTS(@StartYear, 8, 1);
            SET @EndDate = DATEFROMPARTS(@EndYear, 7, 31);
        END
    ;WITH ResultsByAcYear
    AS (
    SELECT ar1.ApprenticeFeedbackTargetId, pa1.AttributeId, pa1.AttributeValue, aft.Ukprn
       FROM (
            SELECT * FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, *
                FROM [dbo].[ApprenticeFeedbackResult] WHERE ((DatetimeCompleted BETWEEN @StartDate AND @EndDate))
            ) ab1 WHERE seq = 1
            ) ar1
            JOIN [dbo].[ProviderAttribute] pa1 on pa1.ApprenticeFeedbackResultId = ar1.Id
            JOIN [dbo].[ApprenticeFeedbackTarget] aft on ar1.ApprenticeFeedbackTargetId = aft.Id
    WHERE FeedbackEligibility != 0 AND ((DatetimeCompleted BETWEEN @StartDate AND @EndDate))
    )
    MERGE INTO [dbo].[ProviderAttributeSummary] pas
    USING (
    SELECT Ukprn, AttributeId
    , SUM(AttributeValue) Agree
    , SUM(CASE WHEN AttributeValue = 1 THEN 0 ELSE 1 END) Disagree
    , GETUTCDATE() UpdatedOn
    FROM (
        SELECT *, COUNT(*) OVER (PARTITION BY Ukprn, AttributeId) ReviewCount
        FROM ResultsByAcYear
    ) ab1
    WHERE ReviewCount >= @minimumNumberOfReviews
    GROUP BY Ukprn, AttributeId
    ) upd
    ON pas.Ukprn = upd.Ukprn AND pas.AttributeId = upd.AttributeId AND pas.TimePeriod = @TimePeriod
    WHEN MATCHED THEN
        UPDATE SET pas.Agree = upd.Agree,
                   pas.Disagree = upd.Disagree,
                   pas.UpdatedOn = upd.UpdatedOn
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Ukprn, AttributeId, Agree, Disagree, UpdatedOn, TimePeriod)
        VALUES (upd.Ukprn, upd.AttributeId, upd.Agree, upd.Disagree, upd.UpdatedOn, @TimePeriod)
     WHEN NOT MATCHED BY SOURCE AND pas.TimePeriod = @TimePeriod THEN
        DELETE;
     FETCH NEXT FROM TimePeriodCursor INTO @TimePeriod;
    END
    CLOSE TimePeriodCursor;
    DEALLOCATE TimePeriodCursor;
END