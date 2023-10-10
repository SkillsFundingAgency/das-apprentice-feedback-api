CREATE TABLE [dbo].[FeedbackTransaction]
(
	[Id] BIGINT NOT NULL IDENTITY PRIMARY KEY,
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER NOT NULL,
	[EmailAddress] NVARCHAR(256),
	[FirstName] NVARCHAR(200),
	[TemplateId] UNIQUEIDENTIFIER,
	[CreatedOn] DATETIME NOT NULL,
	[SendAfter] DATETIME,
	[SentDate] DATETIME,
    [TemplateName] VARCHAR(100) NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_FeedbackTransaction_ApprenticeFeedbackTarget]
ON [dbo].[FeedbackTransaction] ( [ApprenticeFeedbackTargetId] )
INCLUDE ( [SentDate], [TemplateName] );

GO

CREATE NONCLUSTERED INDEX [IX_FeedbackTransaction_CreatedOn]
ON [dbo].[FeedbackTransaction] ( [CreatedOn] )
INCLUDE ( [ApprenticeFeedbackTargetId], [SentDate], [SendAfter], [TemplateName] );

GO

