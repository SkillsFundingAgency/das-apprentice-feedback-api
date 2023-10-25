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
    (4,'Deny_TooLateAfterWithdrawing'),
    (5,'Deny_TooLateAfterPausing'),
    (6,'Deny_HasGivenFeedbackRecently'),
    (7,'Deny_HasGivenFinalFeedback'),
    (8,'Deny_Complete');


MERGE [dbo].[FeedbackEligibilityStatus] TARGET
USING #FeedbackEligibilityStatus SOURCE
ON TARGET.[Id]=SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Id], [Description])
VALUES (SOURCE.[Id],SOURCE.[Description]);

DROP TABLE #FeedbackEligibilityStatus;

