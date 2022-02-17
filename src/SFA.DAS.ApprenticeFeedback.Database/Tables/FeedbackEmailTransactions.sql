CREATE TABLE [dbo].[FeedbackEmailTransactions]
(
    -- Update Id to a sequence
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER, 
    [EmailAddress] NCHAR(256),
    [FirstName] NCHAR(256),
    [EmailTemplateId] UNIQUEIDENTIFIER,
    [CreateOn] DATETIME NOT NULL,
    [SentDate] DATETIME,
)
