CREATE TABLE [dbo].[ApprenticeFeedbackResult]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY, 
    [ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedbackTarget](Id),
    [StandardUId] NVARCHAR(12),
    [DateTimeCompleted] TIMESTAMP, 
    [ProviderRating] NVARCHAR(10)
)
