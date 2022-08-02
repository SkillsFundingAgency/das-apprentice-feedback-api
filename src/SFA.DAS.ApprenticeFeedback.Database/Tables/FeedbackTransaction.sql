CREATE TABLE [dbo].[FeedbackTransaction]
(
	-- Update Id to a sequence
	[Id] int NOT NULL IDENTITY PRIMARY KEY,
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER, 
	[EmailAddress] NCHAR(256),
	[FirstName] NCHAR(256),
	[TemplateId] UNIQUEIDENTIFIER,
	[CreatedOn] DATETIME NOT NULL,
	[SendAfter] DATETIME,
	[SentDate] DATETIME
)
GO

CREATE NONCLUSTERED INDEX [IX_FeedbackTransaction_ApprenticeFeedbackTarget]
ON [dbo].[FeedbackTransaction] ( [ApprenticeFeedbackTargetId] )
INCLUDE ( [SentDate] );

GO
