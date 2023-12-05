CREATE FUNCTION [dbo].[IdentifyOldApprenticeships]
(
    @CurrentUtcDate DATETIME
)
RETURNS TABLE
AS
RETURN
(
    SELECT aft1.[Id]
    FROM [dbo].[ApprenticeFeedbackTarget] aft1
    WHERE aft1.[Status] = 3 -- has been completed
    OR aft1.[Withdrawn] = 1 -- has been withdrawn
    OR aft1.[EndDate] < DATEADD(month, -2, EOMONTH(@CurrentUtcDate)) -- is beyond planned or estimated end date
    --
    UNION
    -- superseded Apprenticeships where the Apprentice has confirmed a more recent Apprenticeship
    SELECT aft2.[Id]
    FROM (
        SELECT [Id]
        ,COUNT(*) OVER (PARTITION BY [ApprenticeId]) HowManyApprenticeships
        ,ROW_NUMBER() OVER (PARTITION BY [ApprenticeId] ORDER BY [CreatedOn] DESC, [ApprenticeshipId] DESC) seqn
        FROM [dbo].[ApprenticeFeedbackTarget]
    ) aft2
    WHERE HowManyApprenticeships > 1
    AND seqn > 1
)
