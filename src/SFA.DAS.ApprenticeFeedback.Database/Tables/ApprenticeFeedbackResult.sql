CREATE TABLE [dbo].[ApprenticeFeedbackResult]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY, 
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedbackTarget](Id),
	[StandardUId] NVARCHAR(12),
	[DateTimeCompleted] DATETIME2, 
	[ProviderRating] NVARCHAR(10), 
    [AllowContact] BIT NULL DEFAULT 0
)
