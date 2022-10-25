CREATE TABLE [dbo].[ApprenticeExitSurvey]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeFeedbackTargetId] UNIQUEIDENTIFIER NOT NULL, 
	[StandardUId] NVARCHAR(12),
	[DateTimeCompleted] DateTime2 NOT NULL,
	[DidNotCompleteApprenticeship] BIT NULL DEFAULT 0,
	[IncompletionReason] NVARCHAR(100),
	[IncompletionFactor_Caring] BIT NULL DEFAULT 0,
	[IncompletionFactor_Family] BIT NULL DEFAULT 0,
	[IncompletionFactor_Financial] BIT NULL DEFAULT 0,
	[IncompletionFactor_Mental] BIT NULL DEFAULT 0,
	[IncompletionFactor_Physical] BIT NULL DEFAULT 0,
	[IncompletionFactor_None] BIT NULL DEFAULT 0,
	[RemainedReason] NVARCHAR(100),
	[ReasonForIncorrect] NVARCHAR(100),
	[AllowContact] BIT NULL DEFAULT 0
)
