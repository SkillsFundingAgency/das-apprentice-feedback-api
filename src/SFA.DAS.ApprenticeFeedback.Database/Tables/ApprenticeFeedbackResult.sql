CREATE TABLE [dbo].[ApprenticeFeedbackResult]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [FeedbackId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedback](FeedbackId),
    [DateTimeCompleted] TIMESTAMP NOT NULL, 
    [ProviderRating] NCHAR(10) NOT NULL
)
