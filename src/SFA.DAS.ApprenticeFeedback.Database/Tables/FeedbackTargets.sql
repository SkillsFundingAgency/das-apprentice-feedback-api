CREATE TABLE [dbo].[FeedbackTargets]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL, 
    [ApprenticeshipId] UNIQUEIDENTIFIER NOT NULL, 
    [Status] NCHAR(10) NULL, 
    [EmailAddress] NCHAR(10) NULL, 
    [FirstName] NCHAR(10) NULL
)
