
CREATE TABLE #ExitSurveyTempAttributes
(
	[AttributeId] INT,
	[AttributeName] NVARCHAR(100),
	[Category] NVARCHAR(100),
	[AttributeType] NVARCHAR(100),
	[Ordering] INT
)

INSERT INTO #ExitSurveyTempAttributes VALUES 
-- Feedback Attributes (Organisation, Support, Communication) 
(1,'Organising well-structured training','Organisation','Feedback',1),
(2,'Communicating clearly with you','Communication','Feedback',1),
(3,'Providing accessible training resources','Organisation','Feedback',2),
(4,'Balancing online learning with classroom-based training to suit your apprenticeship','Organisation','Feedback',3),
(5,'Taking into account your previous learning','Organisation','Feedback',4),
(6,'Helping you learn new skills and develop existing ones','Support','Feedback',1),
(7,'Giving you relevant training that helps you perform your job better','Organisation','Feedback',5),
(8,'Providing Off The Job training that takes up at least 20% of your total apprenticeship time','Organisation','Feedback',6),
(9,'Supporting you and your apprenticeship','Support','Feedback',2),
(10,'Resolving any issues you have','Support','Feedback',3),
(11,'Ensuring you understand the requirements of your end-point assessment','Communication','Feedback',2),
(12,'Preparing you for success in your future career','Support','Feedback',4),

-- Exit Survey v2
-- ApprenticeshipStatus
(13, 'I am currently doing my apprenticeship', 'ApprenticeshipStatus', 'ExitSurvey_v2', 1),
(14, 'I have done my apprenticeship but I am waiting to do my end-point assessment', 'ApprenticeshipStatus', 'ExitSurvey_v2', 2),
(15, 'I have passed my end-point assessment', 'ApprenticeshipStatus', 'ExitSurvey_v2', 3),
(16, 'I am waiting for my employer to appoint me a new training provider', 'ApprenticeshipStatus', 'ExitSurvey_v2', 4),
(17, 'I have left my apprenticeship', 'ApprenticeshipStatus', 'ExitSurvey_v2', 5),
-- PersonalCircumstances
(18, 'I didn''t enjoy the apprenticeship', 'PersonalCircumstances', 'ExitSurvey_v2', 1),
(19, 'I experienced discrimination or poor behaviour', 'PersonalCircumstances', 'ExitSurvey_v2', 2),
(20, 'I had caring responsibilities', 'PersonalCircumstances', 'ExitSurvey_v2', 3),
(21, 'I had family or relationship issues', 'PersonalCircumstances', 'ExitSurvey_v2', 4),
(22, 'I had financial issues', 'PersonalCircumstances', 'ExitSurvey_v2', 5),
(23, 'I had mental health issues', 'PersonalCircumstances', 'ExitSurvey_v2', 6),
(24, 'I had physical health issues', 'PersonalCircumstances', 'ExitSurvey_v2', 7),
(25, 'I was offered another job', 'PersonalCircumstances', 'ExitSurvey_v2', 8),
(26, 'Other personal circumstances', 'PersonalCircumstances', 'ExitSurvey_v2', 9),
-- Employer
(27, 'The job was not what I expected', 'Employer', 'ExitSurvey_v2', 1),
(28, 'The job was too difficult', 'Employer', 'ExitSurvey_v2', 2),
(29, 'The salary was not high enough', 'Employer', 'ExitSurvey_v2', 3),
(30, 'They did not offer reasonable adjustments', 'Employer', 'ExitSurvey_v2', 4),
(31, 'They did not offer suitable training on the job', 'Employer', 'ExitSurvey_v2', 5),
(32, 'They ended my employment', 'Employer', 'ExitSurvey_v2', 6),
(33, 'They transferred me to another apprenticeship', 'Employer', 'ExitSurvey_v2', 7),
(34, 'They were not supportive enough', 'Employer', 'ExitSurvey_v2', 8),
(35, 'Other issues with my employer', 'Employer', 'ExitSurvey_v2', 9),
-- TrainingProvider
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
-- RemainFactors
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
(59, 'None of these would have made me stay', 'RemainFactors', 'ExitSurvey_v2', 13),

-- Exit Survey v1
-- Your Apprenticeship
(90,'I am still doing my apprenticeship','Your Apprenticeship','ExitSurvey_v1',1),
(91,'I am waiting for my employer to appoint me a new training provider','Your Apprenticeship','ExitSurvey_v1',2),
(92,'I completed the apprenticeship and had my end-point assessment','Your Apprenticeship','ExitSurvey_v1',3),
(93,'I completed my training and I''m waiting for the end-point assessment','Your Apprenticeship','ExitSurvey_v1',4),
(94,'I never started an apprenticeship','Your Apprenticeship','ExitSurvey_v1',5),
-- Incompletion Reason
(100,'I did not enjoy it','Incompletion Reason','ExitSurvey_v1',1),
(101,'I had personal or health reasons','Incompletion Reason','ExitSurvey_v1',2),
(102,'I experienced discrimination or poor behaviour','Incompletion Reason','ExitSurvey_v1',3),
(103,'I started another apprenticeship','Incompletion Reason','ExitSurvey_v1',4),
(104,'I started another job','Incompletion Reason','ExitSurvey_v1',5),
(105,'I was made redundant','Incompletion Reason','ExitSurvey_v1',6),
(106,'The job was too difficult','Incompletion Reason','ExitSurvey_v1',7),
(107,'The job was not what I expected','Incompletion Reason','ExitSurvey_v1',8),
(108,'The salary did not meet my financial needs','Incompletion Reason','ExitSurvey_v1',9),
(109,'The training was repetitive','Incompletion Reason','ExitSurvey_v1',10),
(110,'The training was too difficult','Incompletion Reason','ExitSurvey_v1',11),
(111,'The training was of poor quality','Incompletion Reason','ExitSurvey_v1',12),
(112,'The training was not relevant to the job role','Incompletion Reason','ExitSurvey_v1',13),
(113,'The training provider stopped delivering apprenticeships','Incompletion Reason','ExitSurvey_v1',14),
(114,'The training provider ended my apprenticeship','Incompletion Reason','ExitSurvey_v1',15),
(115,'There were issues with my end-point assessment','Incompletion Reason','ExitSurvey_v1',16),
(116,'None of the above','Incompletion Reason','ExitSurvey_v1',17),
-- Incompletion Factor
(120,'Caring responsibilities','Incompletion Factor','ExitSurvey_v1',1),
(121,'Family or relationship issues','Incompletion Factor','ExitSurvey_v1',2),
(122,'Financial issues','Incompletion Factor','ExitSurvey_v1',3),
(123,'Mental health issues','Incompletion Factor','ExitSurvey_v1',4),
(124,'Physical health issues','Incompletion Factor','ExitSurvey_v1',5),
(125,'None of the above','Incompletion Factor','ExitSurvey_v1',6),
-- Reason to remain
(130,'A higher salary','Reason to remain','ExitSurvey_v1',1),
(131,'A mentor or learning coach','Reason to remain','ExitSurvey_v1',2),
(132,'Being able to skip training I have already done','Reason to remain','ExitSurvey_v1',3),
(133,'Better training from my employer','Reason to remain','ExitSurvey_v1',4),
(134,'Better training from my training provider','Reason to remain','ExitSurvey_v1',5),
(135,'More support from my employer','Reason to remain','ExitSurvey_v1',6),
(136,'More support from my training provider','Reason to remain','ExitSurvey_v1',7),
(137,'More information on the end-point assessment process','Reason to remain','ExitSurvey_v1',8),
(138,'More time to undertake learning and training with my training provider','Reason to remain','ExitSurvey_v1',9),
(139,'Outside support to tackle discrimination or other problems','Reason to remain','ExitSurvey_v1',10),
(140,'Reasonable adjustments from my training provider','Reason to remain','ExitSurvey_v1',11),
(141,'None','Reason to remain','ExitSurvey_v1',12);

GO


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