CREATE TABLE [dbo].[FeedbackTransactionClick]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[FeedbackTransactionId] BIGINT NOT NULL,
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER NOT NULL,
	[LinkName] VARCHAR(200) NOT NULL,
	[LinkUrl] NVARCHAR(max) NOT NULL,
	[ClickedOn] DATETIME NOT NULL,
	[CreatedOn] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[UpdatedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
	CONSTRAINT [PK_FeedbackTransactionClick] PRIMARY KEY ([Id])
	CONSTRAINT [FK_FeedbackTransactionClick_FeedbackTransaction] FOREIGN KEY ([FeedbackTransactionId]) REFERENCES [FeedbackTransaction]([Id])
	CONSTRAINT [FK_FeedbackTransactionClick_ApprenticeFeedbackTarget] FOREIGN KEY ([ApprenticeFeedbackTargetId]) REFERENCES [ApprenticeFeedbackTarget]([Id])
)
GO

CREATE NONCLUSTERED INDEX [IX_FeedbackTransactionClick_FeedbackTransactionId]
ON [dbo].[FeedbackTransactionClick] ( [FeedbackTransactionId] , [ApprenticeFeedbackTargetId], [LinkName] )
INCLUDE ( [LinkUrl] , [CreatedOn] );

GO