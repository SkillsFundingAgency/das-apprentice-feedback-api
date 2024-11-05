CREATE PROCEDURE [dbo].[ImportIntoFeedbackTargetVariant_FromFeedbackTargetVariant_Staging]
AS
BEGIN
    MERGE INTO [dbo].[FeedbackTargetVariant] AS TARGET
    USING [dbo].[FeedbackTargetVariant_Staging] AS SOURCE
    ON TARGET.ApprenticeshipId = SOURCE.ApprenticeshipId
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ApprenticeshipId, Variant)
        VALUES (SOURCE.ApprenticeshipId, SOURCE.Variant)
    WHEN MATCHED AND source.Variant IS NOT NULL AND target.Variant != source.Variant THEN
        UPDATE SET target.Variant = source.Variant
    WHEN MATCHED AND source.Variant IS NULL THEN
        DELETE;
    RETURN 0;
END;
