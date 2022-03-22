CREATE TABLE [dbo].[ApprenticeFeedbackTarget]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL, 
    [ApprenticeshipId] BIGINT NOT NULL, 
    [Status] INT NOT NULL,
	[StartDate] DateTime2 NULL,
	[EndDate] DateTime2 NULL

)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ApprenticeFeedbackTarget_ApprenticeIdApprenticeshipId]
    ON [dbo].[ApprenticeFeedbackTarget]
	(
		[ApprenticeId],
		[ApprenticeshipId]
	);