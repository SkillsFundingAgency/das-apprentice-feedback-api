CREATE PROCEDURE [dbo].[TruncateFeedbackTargetVariant_Staging]
AS
BEGIN
    TRUNCATE TABLE [dbo].[FeedbackTargetVariant_Staging];
END;
