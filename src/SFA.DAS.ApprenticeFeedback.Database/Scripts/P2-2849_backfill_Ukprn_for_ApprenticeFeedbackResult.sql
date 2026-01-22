-- fill Ukprn for historical Feedback results
MERGE INTO [dbo].[ApprenticeFeedbackResult] dest
USING
(
SELECT afr.[Id]
      ,aft.[Ukprn]
  FROM [dbo].[ApprenticeFeedbackResult] afr
JOIN [dbo].[ApprenticeFeedbackTarget] aft on aft.Id = afr.ApprenticeFeedbackTargetId
WHERE afr.Ukprn IS NULL
) upd
ON (dest.[Id] = upd.[Id])
WHEN MATCHED THEN
UPDATE SET dest.[Ukprn] = upd.[Ukprn]
;