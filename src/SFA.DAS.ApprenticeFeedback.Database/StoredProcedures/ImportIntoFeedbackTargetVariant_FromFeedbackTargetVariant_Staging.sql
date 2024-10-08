CREATE PROCEDURE [dbo].[ImportIntoFeedbackTargetVariant_FromFeedbackTargetVariant_Staging]
AS
BEGIN
    MERGE INTO [dbo].[FeedbackTargetVariant] AS TARGET
    USING [dbo].[FeedbackTargetVariant_Staging] AS SOURCE
    ON TARGET.ApprenticeshipId = SOURCE.ApprenticeshipId
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ApprenticeshipId, Variant)
        VALUES (SOURCE.ApprenticeshipId, SOURCE.Variant)
    WHEN MATCHED THEN
        UPDATE SET TARGET.Variant = SOURCE.Variant;

    DELETE FROM [dbo].[FeedbackTargetVariant]
    WHERE ApprenticeshipId NOT IN (SELECT ApprenticeshipId FROM [dbo].[FeedbackTargetVariant_Staging]);

    RETURN 0;
END;
