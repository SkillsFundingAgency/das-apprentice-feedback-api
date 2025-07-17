IF(OBJECT_ID('FK_ApprenticeFeedbackTarget_FeedbackTransaction', 'F') IS NULL)
ALTER TABLE [FeedbackTransaction]
ADD CONSTRAINT FK_ApprenticeFeedbackTarget_FeedbackTransaction FOREIGN KEY (ApprenticeFeedbackTargetId)
REFERENCES [ApprenticeFeedbackTarget] (Id);