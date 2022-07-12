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
        WITH 
        ApprenticeFeedbackResult_CTE As(
            SELECT ROW_NUMBER() OVER (PARTITION BY afr.ApprenticeFeedbackTargetId ORDER BY afr.DateTimeCompleted DESC) as RowNumber,
            aft.Ukprn, aft.ProviderName, afr.ProviderRating,
            afr.Id as ApprenticeFeedbackResultId, aft.ApprenticeId, aft.Id as ApprenticeFeedbackTargetId, afr.DateTimeCompleted
              FROM ApprenticeFeedbackTarget aft JOIN ApprenticeFeedbackResult afr
              ON aft.Id = afr.ApprenticeFeedbackTargetId
        ),
        ProviderFeedbackCount_CTE As(
            SELECT Ukprn, Count(Ukprn) as ReviewCount FROM ApprenticeFeedbackResult_CTE
            WHERE RowNumber = 1 AND DateTimeCompleted > @feedbackCutOff
            GROUP BY Ukprn
        )
        SELECT afr.Ukprn, afr.ProviderName, afr.ApprenticeFeedbackTargetId, afr.ApprenticeFeedbackResultId, afr.ApprenticeId,
        afr.ProviderRating, afr.DateTimeCompleted, pfc.ReviewCount
        FROM ApprenticeFeedbackResult_CTE afr JOIN ProviderFeedbackCount_CTE pfc
        ON afr.Ukprn = pfc.Ukprn
        WHERE ReviewCount >= @minimumNumberOfReviews AND RowNumber = 1 
        AND afr.Ukprn in (SELECT Ukprn FROM @ukprns)
    END
GO
