
CREATE VIEW vApprenticeFeedbackResults
AS
SELECT id
    ,CASE [Status] WHEN 0 THEN 'Unknown' WHEN 1 THEN 'Inactive' WHEN 2 THEN 'Active' WHEN 3 THEN 'Completed' END ApprenticeFeedbackStatus
    ,[Ukprn]
    ,[ProviderName]
    ,[StandardName]
    ,CASE [FeedbackEligibility] 
     WHEN 0 THEN 'Unknown'
     WHEN 1 THEN 'Allow'
     WHEN 2 THEN 'Too Soon'
     WHEN 3 THEN 'Too Late After Passing'
     WHEN 4 THEN 'Too Late After Withdrawing'
     WHEN 5 THEN 'Too Late After Pausing'
     WHEN 6 THEN 'Has Given Feedback Recently'
     WHEN 7 THEN 'Has Given Final Feedback'
     WHEN 9 THEN 'Complete'
     ELSE 'Unknown' END FeedbackEligibilityStatus
    ,fbr.*
FROM [dbo].[ApprenticeFeedbackTarget] aft
LEFT JOIN (
    SELECT [ApprenticeFeedbackTargetId]
            ,[DateTimeCompleted]
            ,CASE [ProviderRating] WHEN 'VeryPoor' THEN 'Very Poor' ElSE [ProviderRating]  END [ProviderRating] 
            ,MAX(CASE WHEN at1.attributeid = 1 THEN AttributeName ELSE NULL END) Attribute1Name
            ,MAX(CASE WHEN at1.attributeid = 1 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute1Value
            ,MAX(CASE WHEN at1.attributeid = 2 THEN AttributeName ELSE NULL END) Attribute2Name
            ,MAX(CASE WHEN at1.attributeid = 2 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute2Value
            ,MAX(CASE WHEN at1.attributeid = 3 THEN AttributeName ELSE NULL END) Attribute3Name
            ,MAX(CASE WHEN at1.attributeid = 3 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute3Value
            ,MAX(CASE WHEN at1.attributeid = 4 THEN AttributeName ELSE NULL END) Attribute4Name
            ,MAX(CASE WHEN at1.attributeid = 4 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute4Value
            ,MAX(CASE WHEN at1.attributeid = 5 THEN AttributeName ELSE NULL END) Attribute5Name
            ,MAX(CASE WHEN at1.attributeid = 5 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute5Value
            ,MAX(CASE WHEN at1.attributeid = 6 THEN AttributeName ELSE NULL END) Attribute6Name
            ,MAX(CASE WHEN at1.attributeid = 6 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute6Value
            ,MAX(CASE WHEN at1.attributeid = 7 THEN AttributeName ELSE NULL END) Attribute7Name
            ,MAX(CASE WHEN at1.attributeid = 7 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute7Value
            ,MAX(CASE WHEN at1.attributeid = 8 THEN AttributeName ELSE NULL END) Attribute8Name
            ,MAX(CASE WHEN at1.attributeid = 8 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute8Value
            ,MAX(CASE WHEN at1.attributeid = 9 THEN AttributeName ELSE NULL END) Attribute9Name
            ,MAX(CASE WHEN at1.attributeid = 9 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute9Value
            ,MAX(CASE WHEN at1.attributeid = 10 THEN AttributeName ELSE NULL END) Attribute10Name
            ,MAX(CASE WHEN at1.attributeid = 10 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute10Value
            ,MAX(CASE WHEN at1.attributeid = 11 THEN AttributeName ELSE NULL END) Attribute11Name
            ,MAX(CASE WHEN at1.attributeid = 11 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute11Value
            ,MAX(CASE WHEN at1.attributeid = 12 THEN AttributeName ELSE NULL END) Attribute12Name
            ,MAX(CASE WHEN at1.attributeid = 12 THEN (CASE AttributeValue WHEN 0 THEN 'False' ELSE 'True' END) ELSE NULL END) Attribute12Value

    FROM ( 
      -- get latest feedback for each feedback target
        SELECT * FROM (
            SELECT ROW_NUMBER() OVER (PARTITION BY [ApprenticeFeedbackTargetId] ORDER BY [DateTimeCompleted] DESC) seq, * FROM [dbo].[ApprenticeFeedbackResult]
        ) ab1 WHERE seq = 1 
        ) ar1
        JOIN [dbo].[ProviderAttribute] pa1 on pa1.ApprenticeFeedbackResultId = ar1.Id
        JOIN Attribute at1 on at1.AttributeId = pa1.AttributeId
        GROUP BY [ApprenticeFeedbackTargetId]
              ,[DateTimeCompleted]
              ,[ProviderRating]
    ) fbr on fbr.ApprenticeFeedbackTargetId = aft.Id