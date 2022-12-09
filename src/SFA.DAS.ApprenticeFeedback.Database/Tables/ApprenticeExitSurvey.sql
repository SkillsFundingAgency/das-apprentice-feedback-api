CREATE TABLE [dbo].[ApprenticeExitSurvey]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER NOT NULL, 
	[StandardUId] NVARCHAR(12),
	[DateTimeCompleted] DateTime2 NOT NULL,
	[DidNotCompleteApprenticeship] BIT NULL DEFAULT 0,
	[AllowContact] BIT NULL DEFAULT 0,
	[PrimaryReason] INT NOT NULL DEFAULT 0
)
