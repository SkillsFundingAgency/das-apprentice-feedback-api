CREATE PROCEDURE [dbo].[ImportIntoFeedbackTargetVariant_FromFeedbackTargetVariant_Staging]
AS
BEGIN
     -- the staging table is cleared and repopulated and may contain duplicates, if
     -- there is a duplicate then the latest entry in the order the rows were created
     -- will be taken, i.e. the order they appeared in the imported variant file
     WITH DistinctStaging AS (
        SELECT 
            ApprenticeshipId,
            Variant
        FROM 
        (
            SELECT 
                ApprenticeshipId,
                Variant,
                ROW_NUMBER() OVER (PARTITION BY ApprenticeshipId ORDER BY [Order] DESC) AS seq
            FROM 
                [dbo].[FeedbackTargetVariant_Staging]
        ) AS OrderedStaging
        WHERE seq = 1
    )

    MERGE INTO [dbo].[FeedbackTargetVariant] AS TARGET
    USING DistinctStaging AS SOURCE
    ON TARGET.ApprenticeshipId = SOURCE.ApprenticeshipId
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ApprenticeshipId, Variant)
        VALUES (SOURCE.ApprenticeshipId, SOURCE.Variant)
    WHEN MATCHED AND SOURCE.Variant IS NOT NULL AND TARGET.Variant != SOURCE.Variant THEN
        UPDATE SET TARGET.Variant = SOURCE.Variant
    WHEN MATCHED AND SOURCE.Variant IS NULL THEN
        DELETE;
END;
