CREATE TYPE dbo.UkprnList
AS TABLE
(
  Ukprn BIGINT
);
GO

CREATE PROCEDURE [GetFeedbackForProviders]
    @ukprns As dbo.UkprnList READONLY,
    @recentFeedbackMonths INT,
    @minimumNumberOfReviews INT
AS
    BEGIN
        DECLARE @feedbackCutOff DATE = DATEADD(month, -@recentFeedbackMonths, GETUTCDATE());

	WITH LatestResults 
	AS (
	SELECT ar1.[ApprenticeFeedbackTargetId]
	,ar1.ProviderRating
	,pa1.AttributeId, pa1.AttributeValue, at1.AttributeName, at1.Category, aft.Status ApprenticeFeedbackStatus, aft.Ukprn
	   FROM (
		  -- get latest feedback for each feedback target
			SELECT * FROM (
				SELECT ROW_NUMBER() OVER (PARTITION BY [ApprenticeFeedbackTargetId] ORDER BY [DateTimeCompleted] DESC) seq, * FROM [dbo].[ApprenticeFeedbackResult]
			) ab1 WHERE seq = 1 
			) ar1
			JOIN [dbo].[ProviderAttribute] pa1 on pa1.ApprenticeFeedbackResultId = ar1.Id
			JOIN Attribute at1 on at1.AttributeId = pa1.AttributeId 
			JOIN [dbo].[ApprenticeFeedbackTarget] aft on ar1.ApprenticeFeedbackTargetId = aft.Id
	WHERE FeedbackEligibility != 0 AND DatetimeCompleted >= @feedbackCutOff
	AND Ukprn in (SELECT Ukprn FROM @ukprns))
	SELECT 
	Ukprn, ProviderRating, ProviderRatingCount, AttributeName, Category
	, ProviderAttributeAgree 
	, ProviderAttributeDisagree 
	FROM (
	--
	select *, count(ProviderRating) OVER (PARTITION BY Ukprn, ProviderRating, Attributeid) ProviderRatingCount
	, SUM(AttributeValue) OVER (PARTITION BY Ukprn, AttributeId) ProviderAttributeAgree 
	, SUM(CASE WHEN AttributeValue = 1 THEN 0 ELSE 1 END)OVER (PARTITION BY Ukprn, AttributeId) ProviderAttributeDisagree 
	--
	FROM (
	select *, count(*) OVER (PARTITION BY Ukprn, Attributeid) ReviewCount
	FROM LatestResults
	) ab1
	WHERE ReviewCount >= @minimumNumberOfReviews
	) ab2
	GROUP BY Ukprn, ProviderRating, ProviderRatingCount, AttributeName, Category
	, ProviderAttributeAgree 
	, ProviderAttributeDisagree 
	ORDER BY Ukprn, ProviderRating, AttributeName
END
GO
