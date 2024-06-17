CREATE PROCEDURE [dbo].[GenerateProviderRatingAndStarsSummary]
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

        ;WITH LatestRatings AS (
            SELECT ar1.ApprenticeFeedbackTargetId, ar1.ProviderRating, aft.Ukprn
            FROM (
                SELECT * FROM (
                    SELECT ROW_NUMBER() OVER (PARTITION BY ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, *
                    FROM [dbo].[ApprenticeFeedbackResult]
                ) ab1 WHERE seq = 1
            ) ar1
            JOIN [dbo].[ApprenticeFeedbackTarget] aft ON ar1.ApprenticeFeedbackTargetId = aft.Id
            WHERE FeedbackEligibility != 0
              AND ((DatetimeCompleted BETWEEN @StartDate AND @EndDate))
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
        FETCH NEXT FROM TimePeriodCursor INTO @TimePeriod;
    END
    CLOSE TimePeriodCursor;
    DEALLOCATE TimePeriodCursor;
END