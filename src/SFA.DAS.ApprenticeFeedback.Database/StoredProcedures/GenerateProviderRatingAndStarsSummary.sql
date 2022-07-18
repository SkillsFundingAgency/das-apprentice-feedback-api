-- NOTE limits to filter out of date feedback records and too few responses

CREATE PROCEDURE [dbo].[GenerateProviderRatingAndStarsSummary]
(
    @recentFeedbackMonths INT,
    @minimumNumberOfReviews INT
)
AS
BEGIN

BEGIN TRANSACTION;  

    TRUNCATE TABLE [dbo].[ProviderRatingSummary];
    
    WITH LatestRatings 
    AS (
    SELECT ar1.ApprenticeFeedbackTargetId, ar1.ProviderRating, aft.Ukprn
       FROM (
          -- get latest feedback for each feedback target
            SELECT * FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, * 
                FROM [dbo].[ApprenticeFeedbackResult]
            ) ab1 WHERE seq = 1 
        ) ar1
        JOIN [dbo].[ApprenticeFeedbackTarget] aft on ar1.ApprenticeFeedbackTargetId = aft.Id
    WHERE FeedbackEligibility != 0 AND DatetimeCompleted >= DATEADD(MONTH,-@recentFeedbackMonths,GETUTCDATE())
    )
    -- Get the ratings for all eligble results for each UKPRNS
    INSERT INTO [dbo].[ProviderRatingSummary]  (Ukprn, Rating, RatingCount, UpdatedOn)
    SELECT Ukprn, ProviderRating, COUNT(*) ProviderRatingCount, GETUTCDATE() UpdatedOn
    FROM (
        SELECT *, COUNT(*) OVER (PARTITION BY Ukprn) ReviewCount
        FROM LatestRatings
    ) ab1
    WHERE ReviewCount >= @minimumNumberOfReviews
    GROUP BY Ukprn, ProviderRating;

    TRUNCATE TABLE [dbo].[ProviderStarsSummary];

    -- summarise the rating summary to get the Stars
    INSERT INTO [dbo].[ProviderStarsSummary] (Ukprn, ReviewCount, Stars)
    SELECT Ukprn, ReviewCount, 
    CASE 
    WHEN AvgRating >= 3.5 THEN 4 
    WHEN AvgRating >= 2.5 THEN 3 
    WHEN AvgRating >= 1.5 THEN 2
    ELSE 1 END Stars
    FROM (
        SELECT Ukprn , SUM(RatingCount) ReviewCount,
        ROUND(
            CAST(
                SUM((
                    CASE [Rating] 
                        WHEN 'VeryPoor' THEN 1 
                        WHEN 'Poor' THEN 2 
                        WHEN 'Good' THEN 3 
                        WHEN 'Excellent' THEN 4 
                        ELSE 1 END)
                * RatingCount)
            AS FLOAT) 
            / 
            CAST(SUM(RatingCount) AS FLOAT),1) 
            AvgRating
    FROM [dbo].[ProviderRatingSummary]
    GROUP BY Ukprn
    ) av1

	COMMIT TRANSACTION;  
END
GO
    