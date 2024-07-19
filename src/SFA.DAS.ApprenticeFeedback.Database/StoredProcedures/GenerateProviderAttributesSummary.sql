CREATE PROCEDURE [dbo].[GenerateProviderAttributesSummary]
(
    @recentFeedbackMonths INT,
    @minimumNumberOfReviews INT
)
AS
BEGIN
DECLARE @CurrentDate DATE = GETDATE();
DECLARE @StartYear INT = YEAR(DATEADD(YEAR, -5, @CurrentDate));
DECLARE @EndYear INT = YEAR(@CurrentDate);
DECLARE @TimePeriods TABLE (ID INT IDENTITY(1,1), TimePeriod VARCHAR(10), StartDate DATETIME, EndDate DATETIME);
DECLARE @AcademicStartYear INT;
DECLARE @AcademicEndYear INT;
DECLARE @TimePeriodTemp VARCHAR(10);

WHILE @StartYear <= @EndYear
BEGIN
    IF @CurrentDate <= DATEFROMPARTS(@EndYear, 7, 31)
    BEGIN
        SET @AcademicStartYear = @EndYear - 1;
        SET @AcademicEndYear = @EndYear;
    END
    ELSE
    BEGIN
        SET @AcademicStartYear = @EndYear;
        SET @AcademicEndYear = @EndYear + 1;
    END
	    SET @TimePeriodTemp = CONCAT('AY', RIGHT(CAST(@AcademicStartYear AS VARCHAR), 2), RIGHT(CAST(@AcademicEndYear AS VARCHAR), 2));
        IF NOT EXISTS (SELECT 1 FROM @TimePeriods WHERE TimePeriod = @TimePeriodTemp)
        BEGIN
            INSERT INTO @TimePeriods (TimePeriod, StartDate, EndDate) 
	        VALUES (@TimePeriodTemp,DATETIMEFROMPARTS(@AcademicStartYear, 8, 1, 0, 0, 0, 0), DATETIMEFROMPARTS(@AcademicEndYear, 7, 31, 23, 59, 59, 997));
        END

    SET @EndYear -= 1;
END

DECLARE @TimePeriod VARCHAR(10);
DECLARE @StartDate DATETIME;
DECLARE @EndDate DATETIME;
DECLARE @RowNum INT = 1;
DECLARE @TotalRows INT = (SELECT COUNT(*) FROM @TimePeriods);

WHILE @RowNum <= @TotalRows
BEGIN
    SELECT @TimePeriod = TimePeriod, @StartDate = StartDate, @EndDate = EndDate
    FROM @TimePeriods
    WHERE ID = @RowNum;

	DELETE FROM [dbo].[ProviderAttributeSummary]
    WHERE TimePeriod NOT IN (SELECT TimePeriod FROM @TimePeriods);

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

    SET @RowNum += 1;
END

    ;WITH AllResults AS (
        SELECT Ukprn, AttributeId, 
               SUM(Agree) AS Agree, 
               SUM(Disagree) AS Disagree
        FROM [dbo].[ProviderAttributeSummary]
        WHERE TimePeriod <> 'All'
        GROUP BY Ukprn, AttributeId
    )
    MERGE INTO [dbo].[ProviderAttributeSummary] pas
    USING (
        SELECT Ukprn, AttributeId, Agree, Disagree, GETUTCDATE() AS UpdatedOn
        FROM AllResults
    ) upd
    ON pas.Ukprn = upd.Ukprn AND pas.AttributeId = upd.AttributeId AND pas.TimePeriod = 'All'
    WHEN MATCHED THEN
        UPDATE SET pas.Agree = upd.Agree,
                   pas.Disagree = upd.Disagree,
                   pas.UpdatedOn = upd.UpdatedOn
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Ukprn, AttributeId, Agree, Disagree, UpdatedOn, TimePeriod)
        VALUES (upd.Ukprn, upd.AttributeId, upd.Agree, upd.Disagree, upd.UpdatedOn, 'All')
	WHEN NOT MATCHED BY SOURCE AND pas.TimePeriod = 'All' THEN
        DELETE;
END
