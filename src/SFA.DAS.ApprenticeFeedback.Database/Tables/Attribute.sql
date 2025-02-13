CREATE TABLE [dbo].[Attribute]
(
	[AttributeId] INT 
				NOT NULL 
				IDENTITY(1, 1)
				PRIMARY KEY,
	[AttributeName] NVARCHAR(150) NULL,
	[Category] NVARCHAR(100) NULL,
	[AttributeType] NVARCHAR(100) NULL,
	[Ordering] INT NOT NULL DEFAULT 0
)
