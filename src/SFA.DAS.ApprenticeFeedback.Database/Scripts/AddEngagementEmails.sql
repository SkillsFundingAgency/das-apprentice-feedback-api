-- add /amend the Email Engagement programmes
-- ProgammeType different for new starts and on-programme active 
-- and for short and longer durations
-- 

CREATE TABLE #EngagementEmails
([Id] int not null, 
[ProgrammeType] varchar(20) not null,
[MonthsFromStart] int null,
[MonthsBeforeEnd] int null,
[TemplateName] varchar(100) not null
);

INSERT INTO #EngagementEmails
VALUES 
(1, 'startshort', 0, 24, 'AppStart'),
(2, 'startshort', 1, 6, 'AppPreEPA'),
(4, 'startshort', 3, null, 'AppMonthThree'),
(5, 'startshort', 6, null, 'AppMonthSix'),
(6, 'startshort', 9, null, 'AppMonthNine'),
(7, 'startshort', 12, null, 'AppMonthTwelve'),
(8, 'startshort', 18, null, 'AppMonthEighteen'),


(11, 'startlong', 0, 78, 'AppStart'),
(12, 'startlong', 1, 6, 'AppPreEPA'),
(14, 'startlong', 3, null, 'AppMonthThree'),
(15, 'startlong', 6, null, 'AppMonthSix'),
(16, 'startlong', 9, null, 'AppMonthNine'),
(18, 'startlong', 12, null, 'AppAnnual'),
(19, 'startlong', 24, null, 'AppAnnual'),
(20, 'startlong', 36, null, 'AppAnnual'),
(21, 'startlong', 48, null, 'AppAnnual'),
(22, 'startlong', 60, null, 'AppAnnual'),
(23, 'startlong', 72, null, 'AppAnnual'),

(31, 'activeshort', 0, 24, 'AppWelcome'),
(32, 'activeshort', 1, 6, 'AppPreEPA'),
(34, 'activeshort', 3, null, 'AppMonthThree'),
(35, 'activeshort', 6, null, 'AppMonthSix'),
(36, 'activeshort', 9, null, 'AppMonthNine'),
(37, 'activeshort', 12, null, 'AppMonthTwelve'),
(38, 'activeshort', 18, null, 'AppMonthEighteen'),


(41, 'activelong', 0, 78, 'AppWelcome'),
(42, 'activelong', 1, 6, 'AppPreEPA'),
(44, 'activelong', 3, null, 'AppMonthThree'),
(45, 'activelong', 6, null, 'AppMonthSix'),
(46, 'activelong', 9, null, 'AppMonthNine'),
(48, 'activelong', 12, null, 'AppAnnual'),
(49, 'activelong', 24, null, 'AppAnnual'),
(50, 'activelong', 36, null, 'AppAnnual'),
(51, 'activelong', 48, null, 'AppAnnual'),
(52, 'activelong', 60, null, 'AppAnnual'),
(53, 'activelong', 72, null, 'AppAnnual'),

(54, 'foundation', 0, 8, 'FoundationAppWelcome'),
(55, 'foundation', 1, 6, 'FoundationAppPreEPA'),
(56, 'foundation', 3, null, 'FoundationAppMonthThree'),
(57, 'foundation', 6, null, 'FoundationAppMonthSix');



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

