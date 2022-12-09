CREATE TABLE [dbo].[ExitSurveyAttribute]
(
	[ApprenticeExitSurveyId] UNIQUEIDENTIFIER NOT NULL, 
	[AttributeId] INT NOT NULL, 
	[AttributeValue] INT NOT NULL
	CONSTRAINT PK_ApprenticeExitSurveyIdAttributeId PRIMARY KEY (ApprenticeExitSurveyId, AttributeId)
)
GO

ALTER TABLE [dbo].[ExitSurveyAttribute]
ADD CONSTRAINT FK_ExitSurveyAttribute_ApprenticeExitSurveyId FOREIGN KEY ([ApprenticeExitSurveyId]) REFERENCES [dbo].[ApprenticeExitSurvey](Id)
GO
