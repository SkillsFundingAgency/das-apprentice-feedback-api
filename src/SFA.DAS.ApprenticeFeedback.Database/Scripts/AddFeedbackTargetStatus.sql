-- enums for FeedbackTargetStatus

CREATE TABLE #FeedbackTargetStatus
(
    [Id] INT NOT NULL, 
    [Description] VARCHAR(150) NOT NULL
)

INSERT INTO #FeedbackTargetStatus VALUES 
    (0,'Unknown'),
    (1,'NotYetActive'),
    (2,'Active'),
    (3,'Complete');

MERGE INTO [dbo].[FeedbackTargetStatus] AS TARGET
USING #FeedbackTargetStatus AS SOURCE
ON TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN 
    UPDATE SET TARGET.[Description] = SOURCE.[Description]
WHEN NOT MATCHED BY TARGET THEN 
    INSERT ([Id], [Description])
    VALUES (SOURCE.[Id], SOURCE.[Description]);

DROP TABLE #FeedbackTargetStatus;
