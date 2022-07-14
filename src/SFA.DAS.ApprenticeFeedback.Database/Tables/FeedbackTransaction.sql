CREATE TABLE [dbo].[FeedbackTransaction]
(
	-- Update Id to a sequence
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER, 
	[EmailAddress] NVARCHAR(200),
	[FirstName] NVARCHAR(200),
	[TemplateId] UNIQUEIDENTIFIER,
	[StandardName] NVARCHAR(1000),
	[ProviderName] NVARCHAR(200),
	[CreatedOn] DATETIME NOT NULL,
	[SendAfter] DATETIME,
	[SentDate] DATETIME
)
