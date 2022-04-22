CREATE TABLE [dbo].[ApprenticeFeedbackTarget]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL, 
	[ApprenticeshipId] BIGINT NOT NULL, 
	[Status] INT NOT NULL,
	[StartDate] DateTime2 DEFAULT NULL,
	[EndDate] DateTime2 DEFAULT NULL,
	[Ukprn] BIGINT NULL,
	[ProviderName] NVARCHAR(120),
	[StandardUId] NVARCHAR(12),
	[StandardName] NVARCHAR(120), 
	[FeedbackEligibility] INT NOT NULL DEFAULT 0, 
	[EligibilityCalculationDate] DATETIME2 NULL DEFAULT NULL

)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ApprenticeFeedbackTarget_ApprenticeIdApprenticeshipId]
    ON [dbo].[ApprenticeFeedbackTarget]
	(
		[ApprenticeId],
		[ApprenticeshipId]
	);

GO