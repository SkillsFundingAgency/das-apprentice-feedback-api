CREATE TABLE [dbo].[FeedbackTargetVariant]
(
	[ApprenticeshipId] BIGINT NOT NULL PRIMARY KEY, 
    [Variant] VARCHAR(100) NOT NULL,
    [CreatedOn] DATETIME2(7) DEFAULT GETUTCDATE()
)

