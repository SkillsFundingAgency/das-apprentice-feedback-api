-- enums for FeedbackEligibilityStatus

CREATE TABLE #FeedbackEligibilityStatus
(
    [Id] INT NOT NULL, 
    [Description] VARCHAR(150) NOT NULL
)

INSERT INTO #FeedbackEligibilityStatus VALUES 
    (0,'Unknown'),
    (1,'Allow'),
    (2,'Deny_TooSoon'),
    (3,'__OBSOLETE__'),
    (4,'Deny_TooLateAfterWithdrawing'),
    (5,'Deny_TooLateAfterPausing'),
    (6,'Deny_HasGivenFeedbackRecently'),
    (7,'Deny_HasGivenFinalFeedback'),
    (8,'__OBSOLETE__'),
    (9,'Deny_Complete');

MERGE INTO [dbo].[FeedbackEligibilityStatus] AS TARGET
USING #FeedbackEligibilityStatus AS SOURCE
ON TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN 
    UPDATE SET TARGET.[Description] = SOURCE.[Description]
WHEN NOT MATCHED BY TARGET THEN 
    INSERT ([Id], [Description])
    VALUES (SOURCE.[Id], SOURCE.[Description]);

DROP TABLE #FeedbackEligibilityStatus;
