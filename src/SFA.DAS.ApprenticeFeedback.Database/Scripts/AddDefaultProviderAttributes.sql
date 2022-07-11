
CREATE TABLE #TempAttributes
(
	[AttributeId] INT,
    [AttributeName] NVARCHAR(100),
	[Category] NVARCHAR(100)
)

INSERT INTO #TempAttributes VALUES 
(1, 'Organising well-structured training', 'Organisation'),
(2, 'Communicating clearly with you', 'Communication'),
(3, 'Providing accessible training resources', 'Organisation'), 
(4, 'Balancing online learning with classroom-based training to suit your apprenticeship', 'Organisation'),
(5, 'Taking into account your previous learning', 'Organisation'),
(6, 'Helping you learn new skills and develop existing ones', 'Support'),
(7, 'Giving you relevant training that helps you perform your job better', 'Organisation'),
(8, 'Providing Off The Job training that takes up at least 20% of your total apprenticeship time', 'Organisation'),
(9, 'Supporting you and your apprenticeship', 'Support'),
(10, 'Resolving any issues you have', 'Support'),
(11, 'Ensuring you understand the requirements of your end-point assessment', 'Communication'),
(12, 'Preparing you for success in your future career', 'Support') 


SET IDENTITY_INSERT [dbo].[Attribute] ON;

MERGE Attribute TARGET
USING #TempAttributes SOURCE
ON TARGET.AttributeId=SOURCE.AttributeId
WHEN MATCHED THEN
UPDATE SET TARGET.AttributeName = SOURCE.AttributeName,
Category = SOURCE.Category
WHEN NOT MATCHED BY TARGET THEN 
INSERT (AttributeId,AttributeName, Category)
VALUES (SOURCE.AttributeId,SOURCE.AttributeName, SOURCE.Category);

SET IDENTITY_INSERT [dbo].[Attribute] OFF;
DROP TABLE #TempAttributes