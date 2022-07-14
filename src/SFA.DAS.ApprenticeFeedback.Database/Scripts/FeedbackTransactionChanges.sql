BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_FeedbackTransaction
	(
	Id uniqueidentifier NOT NULL,
	ApprenticeFeedbackTargetId uniqueidentifier NULL,
	EmailAddress nvarchar(200) NULL,
	FirstName nvarchar(200) NULL,
	TemplateId uniqueidentifier NULL,
	StandardName nvarchar(1000) NULL,
	ProviderName nvarchar(200) NULL,
	CreatedOn datetime NOT NULL,
	SendAfter datetime NULL,
	SentDate datetime NULL
	)
GO
ALTER TABLE dbo.Tmp_FeedbackTransaction SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.FeedbackTransaction)
	 EXEC('INSERT INTO dbo.Tmp_FeedbackTransaction (Id, ApprenticeFeedbackTargetId, EmailAddress, FirstName, TemplateId, StandardName, ProviderName, CreatedOn, SendAfter, SentDate)
		SELECT Id, ApprenticeFeedbackTargetId, CONVERT(nvarchar(200), EmailAddress), CONVERT(nvarchar(200), FirstName), EmailTemplateId, StandardName, ProviderName, CreatedOn, SendAfter, SentDate FROM dbo.FeedbackTransaction WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.FeedbackTransaction
GO
EXECUTE sp_rename N'dbo.Tmp_FeedbackTransaction', N'FeedbackTransaction', 'OBJECT' 
GO
ALTER TABLE dbo.FeedbackTransaction ADD CONSTRAINT
	PK__FeedbackTransaction__3214EC0745D27A3E PRIMARY KEY CLUSTERED 
	(
	Id
	)

GO
COMMIT
