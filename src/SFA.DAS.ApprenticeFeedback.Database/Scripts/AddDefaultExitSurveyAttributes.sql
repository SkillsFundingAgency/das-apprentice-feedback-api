
CREATE TABLE #ExitSurveyTempAttributes
(
	[AttributeId] INT,
    [AttributeName] NVARCHAR(100),
	[Category] NVARCHAR(100),
	[AttributeType] NVARCHAR(100),
	[Ordering] INT
)

INSERT INTO #ExitSurveyTempAttributes VALUES 
(13, 'I am currently doing my apprenticeship', 'ApprenticeshipStatus', 'ExitSurvey_v2', 1),
(14, 'I have done my apprenticeship but I am waiting to do my end-point assessment', 'ApprenticeshipStatus', 'ExitSurvey_v2', 2),
(15, 'I have passed my end-point assessment', 'ApprenticeshipStatus', 'ExitSurvey_v2', 3),
(16, 'I am waiting for my employer to appoint me a new training provider', 'ApprenticeshipStatus', 'ExitSurvey_v2', 4),
(17, 'I have left my apprenticeship', 'ApprenticeshipStatus', 'ExitSurvey_v2', 5),

(18, 'I didn''t enjoy the apprenticeship', 'PersonalCircumstances', 'ExitSurvey_v2', 1),
(19, 'I experienced discrimination or poor behaviour', 'PersonalCircumstances', 'ExitSurvey_v2', 2),
(20, 'I had caring responsibilities', 'PersonalCircumstances', 'ExitSurvey_v2', 3),
(21, 'I had family or relationship issues', 'PersonalCircumstances', 'ExitSurvey_v2', 4),
(22, 'I had financial issues', 'PersonalCircumstances', 'ExitSurvey_v2', 5),
(23, 'I had mental health issues', 'PersonalCircumstances', 'ExitSurvey_v2', 6),
(24, 'I had physical health issues', 'PersonalCircumstances', 'ExitSurvey_v2', 7),
(25, 'I was offered another job', 'PersonalCircumstances', 'ExitSurvey_v2', 8),
(26, 'Other personal circumstances', 'PersonalCircumstances', 'ExitSurvey_v2', 9),

(27, 'The job was not what I expected', 'Employer', 'ExitSurvey_v2', 1),
(28, 'The job was too difficult', 'Employer', 'ExitSurvey_v2', 2),
(29, 'The salary was not high enough', 'Employer', 'ExitSurvey_v2', 3),
(30, 'They did not offer reasonable adjustments', 'Employer', 'ExitSurvey_v2', 4),
(31, 'They did not offer suitable training on the job', 'Employer', 'ExitSurvey_v2', 5),
(32, 'They ended my employment', 'Employer', 'ExitSurvey_v2', 6),
(33, 'They transferred me to another apprenticeship', 'Employer', 'ExitSurvey_v2', 7),
(34, 'They were not supportive enough', 'Employer', 'ExitSurvey_v2', 8),
(35, 'Other issues with my employer', 'Employer', 'ExitSurvey_v2', 9),

(36, 'The training was of poor quality', 'TrainingProvider', 'ExitSurvey_v2', 1),
(37, 'The training was not relevant to the job role', 'TrainingProvider', 'ExitSurvey_v2', 2),
(38, 'The training was too difficult', 'TrainingProvider', 'ExitSurvey_v2', 3),
(39, 'The training was too repetitive', 'TrainingProvider', 'ExitSurvey_v2', 4),
(40, 'The training took up too much time', 'TrainingProvider', 'ExitSurvey_v2', 5),
(41, 'They did not offer me reasonable adjustments', 'TrainingProvider', 'ExitSurvey_v2', 6),
(42, 'They stopped delivering apprenticeships', 'TrainingProvider', 'ExitSurvey_v2', 7),
(43, 'They ended my apprenticeship', 'TrainingProvider', 'ExitSurvey_v2', 8),
(44, 'They were not supportive enough', 'TrainingProvider', 'ExitSurvey_v2', 9),
(45, 'I had problems with the end-point assessment', 'TrainingProvider', 'ExitSurvey_v2', 10),
(46, 'Other issues with my training provider', 'TrainingProvider', 'ExitSurvey_v2', 11),

(47, 'A higher salary', 'RemainFactors', 'ExitSurvey_v2', 1),
(48, 'A mentor or learning coach', 'RemainFactors', 'ExitSurvey_v2', 2),
(49, 'Being able to skip repeat training', 'RemainFactors', 'ExitSurvey_v2', 3),
(50, 'Better training from my employer', 'RemainFactors', 'ExitSurvey_v2', 4),
(51, 'Better training from my training provider', 'RemainFactors', 'ExitSurvey_v2', 5),
(52, 'More support from my employer', 'RemainFactors', 'ExitSurvey_v2', 6),
(53, 'More support from my training provider', 'RemainFactors', 'ExitSurvey_v2', 7),
(54, 'More information on the end-point assessment process', 'RemainFactors', 'ExitSurvey_v2', 8),
(55, 'More time to undertake learning and training with my training provider', 'RemainFactors', 'ExitSurvey_v2', 9),
(56, 'Outside support to tackle discrimination or other problems', 'RemainFactors', 'ExitSurvey_v2', 10),
(57, 'Reasonable adjustments from my employer', 'RemainFactors', 'ExitSurvey_v2', 11),
(58, 'Reasonable adjustments from my training provider', 'RemainFactors', 'ExitSurvey_v2', 12),
(59, 'None of these would have made me stay', 'RemainFactors', 'ExitSurvey_v2', 13)

SET IDENTITY_INSERT [dbo].[Attribute] ON;

MERGE Attribute TARGET
USING #ExitSurveyTempAttributes SOURCE
ON TARGET.AttributeId=SOURCE.AttributeId
WHEN MATCHED THEN
UPDATE SET TARGET.AttributeName = SOURCE.AttributeName,
Category = SOURCE.Category,
AttributeType = SOURCE.AttributeType,
Ordering = SOURCE.Ordering
WHEN NOT MATCHED BY TARGET THEN 
INSERT (AttributeId,AttributeName, Category, AttributeType, Ordering)
VALUES (SOURCE.AttributeId,SOURCE.AttributeName, SOURCE.Category, SOURCE.AttributeType, SOURCE.Ordering);

SET IDENTITY_INSERT [dbo].[Attribute] OFF;
DROP TABLE #ExitSurveyTempAttributes