CREATE TABLE [dbo].[ApprenticeFeedbackTarget]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL, 
	[ApprenticeshipId] BIGINT NOT NULL, 
	[Status] INT NOT NULL,
	[StartDate] DateTime2 DEFAULT NULL,
	[EndDate] DateTime2 DEFAULT NULL,
	[Ukprn] BIGINT NULL,
	[ProviderName] NVARCHAR(10),
	[StandardUId] NVARCHAR(12),
	[StandardName] NVARCHAR(100), 
    [FeedbackEligibility] INT NULL DEFAULT NULL, 
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