MERGE INTO [dbo].[ApprenticeExitSurvey] aes
USING (
SELECT [ApprenticeExitSurveyId]
  FROM [dbo].[ExitSurveyAttribute]
  WHERE [AttributeId] = 17
  ) upd
ON aes.[Id] = upd.[ApprenticeExitSurveyId]
WHEN MATCHED AND aes.[DidNotCompleteApprenticeship] = 0 
THEN UPDATE SET aes.[DidNotCompleteApprenticeship] = 1
;