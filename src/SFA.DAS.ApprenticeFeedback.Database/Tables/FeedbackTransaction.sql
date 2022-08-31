CREATE TABLE [dbo].[FeedbackTransaction]
(
	[Id] BIGINT NOT NULL IDENTITY PRIMARY KEY,
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER NOT NULL,
	[EmailAddress] NVARCHAR(256),
	[FirstName] NVARCHAR(200),
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
