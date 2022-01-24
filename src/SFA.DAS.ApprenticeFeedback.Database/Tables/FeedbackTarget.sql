CREATE TABLE [dbo].[FeedbackTarget]
(
	[ApprenticeId] INT NOT NULL PRIMARY KEY, 
    [ApprenticeshipId] NCHAR(10) NULL, 
    [Status] NCHAR(10) NULL, 
    [EmailAddress] NCHAR(10) NULL, 
    [FirstName] NCHAR(10) NULL
)
