CREATE TABLE [dbo].[ProviderAttribute]
(
	[ApprenticeFeedbackResultId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedbackResult]([ApprenticeFeedbackResultId]), 
    [AttributeId] INT NULL, 
    [AttributeValue] INT NULL
)
