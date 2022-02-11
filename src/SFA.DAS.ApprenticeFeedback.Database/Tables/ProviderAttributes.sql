CREATE TABLE [dbo].[ProviderAttributes]
(
	[EmployerFeedbackResultId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedbackResult](Id), 
    [AttributeId] NCHAR(10) NULL, 
    [AttributeValue] NCHAR(10) NULL
)
