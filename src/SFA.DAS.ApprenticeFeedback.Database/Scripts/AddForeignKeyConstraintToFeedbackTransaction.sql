ALTER TABLE [FeedbackTransaction]
ADD CONSTRAINT FK_ApprenticeFeedbackTarget_FeedbackTransaction FOREIGN KEY (ApprenticeFeedbackTargetId)
REFERENCES [ApprenticeFeedbackTarget] (Id);