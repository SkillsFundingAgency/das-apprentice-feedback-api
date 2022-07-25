CREATE TABLE [dbo].[FeedbackTransaction]
(
	-- Update Id to a sequence
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER, 
	[EmailAddress] NCHAR(256),
	[FirstName] NCHAR(256),
	[TemplateId] UNIQUEIDENTIFIER,
	[StandardName] NVARCHAR(100),
	[ProviderName] NVARCHAR(12),
	[CreatedOn] DATETIME NOT NULL,
	[SendAfter] DATETIME,
	[SentDate] DATETIME
)
