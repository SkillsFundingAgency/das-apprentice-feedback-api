CREATE TABLE [dbo].[ApprenticeFeedbackResult]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY, 
    [ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedbackTarget](Id),
    [Ukprn] BIGINT,
    [StandardReference] NVARCHAR(6),
    [LarsCode] BIGINT,
    [StandardUId] NVARCHAR(12),
    [DateTimeCompleted] TIMESTAMP, 
    [ProviderRating] NVARCHAR(10)
)
