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
        UPDATE SET TARGET.Variant = SOURCE.Variant
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

    RETURN 0;
END;
