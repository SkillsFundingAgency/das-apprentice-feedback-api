CREATE PROCEDURE [dbo].[GenerateProviderRatingAndStarsSummary]
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

WHILE @RowNum <= @TotalRows
BEGIN
    SELECT @TimePeriod = TimePeriod, @StartDate = StartDate, @EndDate = EndDate
    FROM @TimePeriods
    WHERE ID = @RowNum;
	
	DELETE FROM [dbo].[ProviderRatingSummary]
    WHERE TimePeriod NOT IN (SELECT TimePeriod FROM @TimePeriods);

        ;WITH LatestRatings AS (
            SELECT ar1.ApprenticeFeedbackTargetId, ar1.ProviderRating, aft.Ukprn
            FROM (
                SELECT * FROM (
                    SELECT ROW_NUMBER() OVER (PARTITION BY ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, *
                    FROM [dbo].[ApprenticeFeedbackResult] WHERE ((DateTimeCompleted BETWEEN @StartDate AND @EndDate))
                ) ab1 WHERE seq = 1
            ) ar1
            JOIN [dbo].[ApprenticeFeedbackTarget] aft ON ar1.ApprenticeFeedbackTargetId = aft.Id
            WHERE FeedbackEligibility != 0
              AND ((DateTimeCompleted BETWEEN @StartDate AND @EndDate))
        )

	MERGE INTO [dbo].[ProviderRatingSummary]  prs
    USING (
    SELECT Ukprn, ProviderRating Rating, COUNT(*) RatingCount, GETUTCDATE() UpdatedOn
    FROM (
        SELECT *, COUNT(*) OVER (PARTITION BY Ukprn) ReviewCount
        FROM LatestRatings
    ) ab1
	WHERE ReviewCount >= @minimumNumberOfReviews
    GROUP BY Ukprn, ProviderRating
    ) upd
        ON prs.Ukprn = upd.Ukprn AND prs.Rating = upd.Rating AND prs.TimePeriod = @TimePeriod
        WHEN MATCHED THEN
            UPDATE SET prs.RatingCount = upd.RatingCount,
                       prs.UpdatedOn = upd.UpdatedOn
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (Ukprn, Rating, RatingCount, UpdatedOn, TimePeriod)
            VALUES (upd.Ukprn, upd.Rating, upd.RatingCount, upd.UpdatedOn, @TimePeriod)
        WHEN NOT MATCHED BY SOURCE AND prs.TimePeriod = @TimePeriod THEN
            DELETE;
        MERGE INTO [dbo].[ProviderStarsSummary] pss
        USING (
            SELECT Ukprn, ReviewCount,
                CASE
                WHEN AvgRating >= 3.5 THEN 4
                WHEN AvgRating >= 2.5 THEN 3
                WHEN AvgRating >= 1.5 THEN 2
                ELSE 1 END Stars
            FROM (
                SELECT Ukprn, SUM(RatingCount) ReviewCount,
                    ROUND(CAST(SUM((CASE [Rating] WHEN 'VeryPoor' THEN 1 WHEN 'Poor' THEN 2 WHEN 'Good' THEN 3 WHEN 'Excellent' THEN 4 ELSE 1 END) * RatingCount) AS FLOAT) / CAST(SUM(RatingCount) AS FLOAT), 1) AvgRating
                FROM [dbo].[ProviderRatingSummary]
                WHERE TimePeriod = @TimePeriod
                GROUP BY Ukprn
            ) av1
        ) upd
        ON pss.Ukprn = upd.Ukprn AND pss.TimePeriod = @TimePeriod
        WHEN MATCHED THEN
            UPDATE SET pss.ReviewCount = upd.ReviewCount,
                       pss.Stars = upd.Stars
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (Ukprn, ReviewCount, Stars, TimePeriod)
            VALUES (upd.Ukprn, upd.ReviewCount, upd.Stars, @TimePeriod)
WHEN NOT MATCHED BY SOURCE AND pss.TimePeriod = @TimePeriod THEN
            DELETE;  

    SET @RowNum += 1;
END

-- Handle 'All' condition outside the loop
DELETE FROM [dbo].[ProviderStarsSummary]
	WHERE TimePeriod NOT IN (SELECT TimePeriod FROM @TimePeriods) AND TimePeriod != 'All';
BEGIN
    ;WITH ProviderRatingsWithTolerance AS (
        SELECT
            Ukprn,
            SUM(ReviewCount) AS ReviewCount,
			ROUND(AVG(CAST(Stars AS FLOAT)), 1) AS AvgRating
        FROM
            [dbo].[ProviderStarsSummary]
        WHERE TimePeriod != 'All'
        GROUP BY
            Ukprn
    )
    MERGE INTO [dbo].[ProviderStarsSummary] pss
    USING (
        SELECT 
            Ukprn,
            ReviewCount,
            ROUND(AvgRating, 0) AS Stars
        FROM ProviderRatingsWithTolerance
    ) upd
    ON pss.Ukprn = upd.Ukprn AND pss.TimePeriod = 'All'
    WHEN MATCHED THEN 
        UPDATE SET pss.ReviewCount = upd.ReviewCount, pss.Stars = upd.Stars
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, ReviewCount, Stars, TimePeriod)
        VALUES (upd.Ukprn, upd.ReviewCount, upd.Stars, 'All')
    WHEN NOT MATCHED BY SOURCE AND pss.TimePeriod = 'All' THEN
        DELETE;
END
END
