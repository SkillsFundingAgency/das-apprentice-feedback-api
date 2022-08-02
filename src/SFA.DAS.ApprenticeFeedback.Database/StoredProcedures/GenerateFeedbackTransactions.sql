CREATE PROCEDURE [dbo].[GenerateFeedbackTransactions]

AS

DECLARE @CreatedOn datetime
SELECT @CreatedOn = GETUTCDATE()

INSERT INTO dbo.FeedbackTransaction (ApprenticeFeedbackTargetId, CreatedOn)
SELECT aft.Id, @CreatedOn
FROM [dbo].[ApprenticeFeedbackTarget] aft
LEFT JOIN (SELECT ApprenticeFeedbackTargetId
           FROM [dbo].[FeedbackTransaction] 
           WHERE SentDate IS NULL OR (SentDate IS NOT NULL AND SentDate >= DATEADD(day,-90,GETDATE())) ) ft1
    on ft1.ApprenticeFeedbackTargetId = aft.Id
WHERE Status = 2 -- "active"
AND FeedbackEligibility = 1 -- "allow"
AND ft1.ApprenticeFeedbackTargetId IS NULL

SELECT @@ROWCOUNT AS Count, @CreatedOn AS CreatedOn

GO
