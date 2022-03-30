--write sql script to populate provider attributes in the table
SET IDENTITY_INSERT Attribute ON;
GO

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '1', 'Organising well-structured training') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Organising well-structured training'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '2', 'Communicating clearly with you') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Communicating clearly with you'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '3', 'Providing accessible training resources') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Providing accessible training resources'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '4', 'Balancing online learning with classroom-based training to suit your apprenticeship') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Balancing online learning with classroom-based training to suit your apprenticeship'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '5', 'Taking into account your previous learning') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Taking into account your previous learning'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '6', 'Helping you learn new skills and develop existing ones') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Helping you learn new skills and develop existing ones'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '7', 'Giving you relevant training that helps you perform your job better') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Giving you relevant training that helps you perform your job better'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '8', 'Training that takes up at least 20% of your total apprenticeship time') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Training that takes up at least 20% of your total apprenticeship time'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '9', 'Supporting you and your apprenticeship') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Supporting you and your apprenticeship'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '10', 'Responding to your needs') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Responding to your needs'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '11', 'Ensuring you understand the requirements of your end-point assessment') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Ensuring you understand the requirements of your end-point assessment'
	)

INSERT INTO Attribute(AttributeId, AttributeName)
SELECT *
FROM (SELECT '12', 'Preparing you for success in your future career') AS tmp(AttributeId, AttributeName)
WHERE NOT EXISTS (
	SELECT AttributeName 
	FROM Attribute
	WHERE AttributeName = 'Preparing you for success in your future career'
	)

SET IDENTITY_INSERT Attribute OFF;
GO