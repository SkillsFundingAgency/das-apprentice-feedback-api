-- Insert data into the Attribute
SET IDENTITY_INSERT [dbo].[Attribute]  ON	
INSERT INTO [dbo].[Attribute]  (
    AttributeId,
    AttributeName,
    Category,
    AttributeType,
    Ordering
) VALUES
(1, 'Organising well-structured training', 'Organisation', 'Feedback', 1),
(2, 'Communicating clearly with you', 'Communication', 'Feedback', 1),
(3, 'Providing accessible training resources', 'Organisation', 'Feedback', 2),
(4, 'Balancing online learning with classroom-based training to suit your apprenticeship', 'Organisation', 'Feedback', 3),
(5, 'Taking into account your previous learning', 'Organisation', 'Feedback', 4),
(6, 'Helping you learn new skills and develop existing ones', 'Support', 'Feedback', 1),
(7, 'Giving you relevant training that helps you perform your job better', 'Organisation', 'Feedback', 5),
(8, 'Providing Off The Job training that takes up at least 20% of your total apprenticeship time', 'Organisation', 'Feedback', 6),
(9, 'Supporting you and your apprenticeship', 'Support', 'Feedback', 2),
(10, 'Resolving any issues you have', 'Support', 'Feedback', 3),
(11, 'Ensuring you understand the requirements of your end-point assessment', 'Communication', 'Feedback', 2),
(12, 'Preparing you for success in your future career', 'Support', 'Feedback', 4);
SET IDENTITY_INSERT [dbo].[Attribute] OFF