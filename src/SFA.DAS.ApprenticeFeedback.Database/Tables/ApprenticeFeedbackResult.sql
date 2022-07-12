CREATE TABLE [dbo].[ApprenticeFeedbackResult]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY, 
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedbackTarget](Id),
	[StandardUId] NVARCHAR(12),
	[DateTimeCompleted] DATETIME2, 
	[ProviderRating] NVARCHAR(10), 
    [AllowContact] BIT NULL DEFAULT 0
)

GO

CREATE NONCLUSTERED INDEX [IX_ApprenticeFeedbackResult_ApprenticeFeedbackTarget]
    ON [dbo].[ApprenticeFeedbackResult]
	(		[ApprenticeFeedbackTargetId],[DateTimeCompleted]	) INCLUDE ( [ProviderRating] ) 

GO
