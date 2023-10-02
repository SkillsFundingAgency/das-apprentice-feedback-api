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
    


MERGE [dbo].[FeedbackTargetStatus] TARGET
USING #FeedbackTargetStatus SOURCE
ON TARGET.[Id]=SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Id], [Description])
VALUES (SOURCE.[Id],SOURCE.[Description]);

DROP TABLE #FeedbackTargetStatus;
