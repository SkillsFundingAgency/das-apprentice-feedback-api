-- NOTE limits to filter out of date feedback records and too few responses

CREATE PROCEDURE [dbo].[GenerateProviderRatingAndStarsSummary]
(
    @recentFeedbackMonths INT,
    @minimumNumberOfReviews INT
)
AS
BEGIN
    
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
    ON prs.Ukprn = upd.Ukprn AND prs.Rating = upd.Rating
    WHEN MATCHED THEN 
        UPDATE SET prs.RatingCount = upd.RatingCount, 
                   prs.UpdatedOn = upd.UpdatedOn
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, Rating, RatingCount, UpdatedOn)
        VALUES (upd.Ukprn, upd.Rating, upd.RatingCount, upd.UpdatedOn)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;
 
    -- summarise the rating summary to get the Stars
    MERGE INTO [dbo].[ProviderStarsSummary] pss
    USING (
    SELECT Ukprn, ReviewCount, 
        CASE 
        WHEN AvgRating >= 3.5 THEN 4 
        WHEN AvgRating >= 2.5 THEN 3 
        WHEN AvgRating >= 1.5 THEN 2
        ELSE 1 END Stars
    FROM (
        SELECT Ukprn , SUM(RatingCount) ReviewCount,
        ROUND(CAST( SUM((
                    CASE [Rating] 
                    WHEN 'VeryPoor' THEN 1 
                    WHEN 'Poor' THEN 2 
                    WHEN 'Good' THEN 3 
                    WHEN 'Excellent' THEN 4 
                    ELSE 1 END)
                * RatingCount) AS FLOAT) / CAST(SUM(RatingCount) AS FLOAT),1) AvgRating
        FROM [dbo].[ProviderRatingSummary]
        GROUP BY Ukprn
        ) av1
    ) upd
    ON pss.Ukprn = upd.Ukprn
    WHEN MATCHED THEN 
        UPDATE SET pss.ReviewCount = upd.ReviewCount, 
                   pss.Stars = upd.Stars
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, ReviewCount, Stars)
        VALUES (upd.Ukprn, upd.ReviewCount, upd.Stars)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;    
  
END
GO
