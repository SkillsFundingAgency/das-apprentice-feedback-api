﻿-- NOTE limits to filter out of date feedback records and too few responses

CREATE PROCEDURE [dbo].[GenerateProviderAttributesSummary]
(
    @recentFeedbackMonths INT,
    @minimumNumberOfReviews INT
)
AS
BEGIN

   
    WITH LatestResults 
    AS (
    SELECT ar1.ApprenticeFeedbackTargetId, pa1.AttributeId, pa1.AttributeValue, aft.Ukprn
       FROM (
          -- get latest feedback for each feedback target
            SELECT * FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY ApprenticeFeedbackTargetId ORDER BY DateTimeCompleted DESC) seq, * 
                FROM [dbo].[ApprenticeFeedbackResult]
            ) ab1 WHERE seq = 1 
            ) ar1
            JOIN [dbo].[ProviderAttribute] pa1 on pa1.ApprenticeFeedbackResultId = ar1.Id
            JOIN [dbo].[ApprenticeFeedbackTarget] aft on ar1.ApprenticeFeedbackTargetId = aft.Id
    WHERE FeedbackEligibility != 0 AND DatetimeCompleted >= DATEADD(MONTH,-@recentFeedbackMonths,GETUTCDATE())
    )
    -- Get the ratings for all eligble results for each UKPRNS
    MERGE INTO [dbo].[ProviderAttributeSummary] pas 
    USING (    
    SELECT Ukprn, AttributeId
    , SUM(AttributeValue) Agree 
    , SUM(CASE WHEN AttributeValue = 1 THEN 0 ELSE 1 END) Disagree 
    , GETUTCDATE() UpdatedOn
    FROM (
        SELECT *, COUNT(*) OVER (PARTITION BY Ukprn, AttributeId) ReviewCount
        FROM LatestResults
    ) ab1
    WHERE ReviewCount >= @minimumNumberOfReviews
    GROUP BY Ukprn, AttributeId
    ) upd
    ON pas.Ukprn = upd.Ukprn AND pas.AttributeId = upd.AttributeId
    WHEN MATCHED THEN 
        UPDATE SET pas.Agree = upd.Agree, 
                   pas.Disagree = upd.Disagree,
                   pas.UpdatedOn = upd.UpdatedOn
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, AttributeId, Agree, Disagree, UpdatedOn) 
        VALUES (upd.Ukprn, upd.AttributeId, upd.Agree, upd.Disagree, upd.UpdatedOn)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

    
END
GO
    