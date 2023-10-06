-- add /amend the Email Engagement programmes

CREATE TABLE #EngagementEmails
([Id] int not null, 
[ProgrammeType] varchar(20) not null,
[MonthsFromStart] int null,
[MonthsBeforeEnd] int null,
[TemplateName] varchar(100) not null
);

INSERT INTO #EngagementEmails
VALUES 
(1, 'short', 0, null, 'AppStart'),
(2, 'short', 1, 6, 'AppPreEPA'),
(4, 'short', 3, null, 'AppMonthThree'),
(5, 'short', 6, null, 'AppMonthSix'),
(6, 'short', 9, null, 'AppMonthNine'),
(7, 'short', 12, null, 'AppMonthTwelve'),
(8, 'short', 18, null, 'AppMonthEighteen'),


(11, 'long', 0, null, 'AppStart'),
(12, 'long', 1, 6, 'AppPreEPA'),
(14, 'long', 3, null, 'AppMonthThree'),
(15, 'long', 6, null, 'AppMonthSix'),
(16, 'long', 9, null, 'AppMonthNine'),
(18, 'long', 12, null, 'AppAnnual'),
(19, 'long', 24, null, 'AppAnnual'),
(20, 'long', 36, null, 'AppAnnual'),
(21, 'long', 48, null, 'AppAnnual'),
(22, 'long', 60, null, 'AppAnnual'),
(23, 'long', 72, null, 'AppAnnual');



MERGE [dbo].[EngagementEmails] TARGET
USING #EngagementEmails SOURCE
ON (TARGET.[Id]=SOURCE.[Id])
WHEN MATCHED THEN
UPDATE SET TARGET.[ProgrammeType] = SOURCE.[ProgrammeType],
TARGET.[MonthsFromStart] = SOURCE.[MonthsFromStart],
TARGET.[MonthsBeforeEnd] = SOURCE.[MonthsBeforeEnd],
TARGET.[TemplateName] = SOURCE.[TemplateName]
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, ProgrammeType, MonthsFromStart, MonthsBeforeEnd, TemplateName)
VALUES (SOURCE.Id, SOURCE.ProgrammeType, SOURCE.MonthsFromStart, SOURCE.MonthsBeforeEnd, SOURCE.TemplateName)
WHEN NOT MATCHED BY SOURCE THEN
DELETE;

DROP TABLE #EngagementEmails;

