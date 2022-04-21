CREATE TABLE [dbo].[ProviderAttribute]
(
	[ApprenticeFeedbackResultId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [dbo].[ApprenticeFeedbackResult](Id), 
    [AttributeId] INT NOT NULL, 
    [AttributeValue] INT NOT NULL
    CONSTRAINT PK_ApprenticeFeedbackResultIdAttributeId PRIMARY KEY (ApprenticeFeedbackResultId, AttributeId)
)
