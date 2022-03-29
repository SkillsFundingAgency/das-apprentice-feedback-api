CREATE TABLE [dbo].[ApprenticeFeedbackTargets]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL, 
    [ApprenticeshipId] BIGINT NULL, 
    [Status] INT NULL
)
