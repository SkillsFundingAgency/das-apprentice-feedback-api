CREATE TABLE [dbo].[EngagementEmails]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[ProgrammeType] VARCHAR(20) NOT NULL,
	[MonthsFromStart] INT NULL,
	[MonthsBeforeEnd] INT NULL,
	[TemplateName] VARCHAR(100) NOT NULL
);
