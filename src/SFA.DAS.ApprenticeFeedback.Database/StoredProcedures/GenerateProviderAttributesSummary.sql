CREATE PROCEDURE [dbo].[GenerateProviderAttributesSummary]
(
    @recentFeedbackMonths INT, --The parameter is not being used. It should be removed
    @minimumNumberOfReviews INT
)
AS
BEGIN
DECLARE @CurrentDate DATE = GETDATE();
DECLARE @CurrentYear INT = YEAR(@CurrentDate);
DECLARE @StartYear INT = YEAR(DATEADD(YEAR, -5, @CurrentDate));
DECLARE @EndYear INT = YEAR(@CurrentDate);
DECLARE @TimePeriods TABLE (ID INT IDENTITY(1,1), TimePeriod VARCHAR(10), StartDate DATETIME, EndDate DATETIME);
DECLARE @AcademicStartYear INT;
DECLARE @AcademicEndYear INT;
DECLARE @TimePeriodTemp VARCHAR(10);

IF @CurrentDate <= DATEFROMPARTS(@CurrentYear, 7, 31)
BEGIN
    SET @EndYear = @CurrentYear;
    SET @StartYear = @EndYear - 5;
END
ELSE
BEGIN
    SET @EndYear = @CurrentYear + 1;
    SET @StartYear = @EndYear - 5;
END

WHILE @StartYear < @EndYear
BEGIN

    SET @AcademicStartYear = @StartYear;
    SET @AcademicEndYear = @StartYear + 1;

	    SET @TimePeriodTemp = CONCAT('AY', RIGHT(CAST(@AcademicStartYear AS VARCHAR), 2), RIGHT(CAST(@AcademicEndYear AS VARCHAR), 2));
        IF NOT EXISTS (SELECT 1 FROM @TimePeriods WHERE TimePeriod = @TimePeriodTemp)
        BEGIN
            INSERT INTO @TimePeriods (TimePeriod, StartDate, EndDate) 
	        VALUES (@TimePeriodTemp,DATETIMEFROMPARTS(@AcademicStartYear, 8, 1, 0, 0, 0, 0), DATETIMEFROMPARTS(@AcademicEndYear, 7, 31, 23, 59, 59, 997));
        END

       SET @StartYear += 1;
END

DECLARE @TimePeriod VARCHAR(10);
DECLARE @StartDate DATETIME;
DECLARE @EndDate DATETIME;
DECLARE @RowNum INT = 1;
DECLARE @TotalRows INT = (SELECT COUNT(*) FROM @TimePeriods);

DELETE FROM [dbo].[ProviderAttributeSummary]
WHERE TimePeriod NOT IN (SELECT TimePeriod FROM @TimePeriods) AND TimePeriod != 'All';

WHILE @RowNum <= @TotalRows
BEGIN
    SELECT @TimePeriod = TimePeriod, @StartDate = StartDate, @EndDate = EndDate
    FROM @TimePeriods
    WHERE ID = @RowNum;

    ;WITH ResultsByAcYear
    AS (
    SELECT ar1.ApprenticeFeedbackTargetId, pa1.AttributeId, pa1.AttributeValue, aft.Ukprn
       FROM (
            SELECT * FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, *
                FROM [dbo].[ApprenticeFeedbackResult] WHERE ((DateTimeCompleted BETWEEN @StartDate AND @EndDate))
            ) ab1 WHERE seq = 1
            ) ar1
            JOIN [dbo].[ProviderAttribute] pa1 on pa1.ApprenticeFeedbackResultId = ar1.Id
            JOIN [dbo].[ApprenticeFeedbackTarget] aft on ar1.ApprenticeFeedbackTargetId = aft.Id
    WHERE FeedbackEligibility != 0 AND ((DateTimeCompleted BETWEEN @StartDate AND @EndDate))
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
BEGIN
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
END
