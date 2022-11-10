CREATE TABLE [dbo].[ExitSurveyAttribute]
(
	[ApprenticeExitSurveyId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [dbo].[ApprenticeExitSurvey](Id), 
    [AttributeId] INT NOT NULL, 
    [AttributeValue] INT NOT NULL
    CONSTRAINT PK_ApprenticeExitSurveyIdAttributeId PRIMARY KEY (ApprenticeExitSurveyId, AttributeId)
)
