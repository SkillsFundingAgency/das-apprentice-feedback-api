
CREATE TABLE #TempAttributes
(
	[AttributeId] INT ,
    [AttributeName] NVARCHAR(100)
)

INSERT INTO #TempAttributes VALUES 
(1, 'Organising well-structured training'),
(2, 'Communicating clearly with you'),
(3, 'Providing accessible training resources'), 
(4, 'Balancing online learning with classroom-based training to suit your apprenticeship'),
(5, 'Taking into account your previous learning'),
(6, 'Helping you learn new skills and develop existing ones'),
(7, 'Giving you relevant training that helps you perform your job better'),
(8, 'Providing Off The Job training that takes up at least 20% of your total apprenticeship time'),
(9, 'Supporting you and your apprenticeship'),
(10, 'Resolving any issues you have'),
(11, 'Ensuring you understand the requirements of your end-point assessment'),
(12, 'Preparing you for success in your future career') 


SET IDENTITY_INSERT [dbo].[Attribute] ON;

MERGE Attribute TARGET
USING #TempAttributes SOURCE
ON TARGET.AttributeId=SOURCE.AttributeId
WHEN MATCHED THEN
UPDATE SET TARGET.AttributeName = SOURCE.AttributeName
WHEN NOT MATCHED BY TARGET THEN 
INSERT (AttributeId,AttributeName)
VALUES (SOURCE.AttributeId,SOURCE.AttributeName);

SET IDENTITY_INSERT [dbo].[Attribute] OFF;
DROP TABLE #TempAttributes

