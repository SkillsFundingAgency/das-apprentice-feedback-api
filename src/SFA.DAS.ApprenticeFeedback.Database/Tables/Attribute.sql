CREATE TABLE [dbo].[Attribute]
(
	[AttributeId] INT 
				NOT NULL 
				IDENTITY(1, 1)
				PRIMARY KEY,
    [AttributeName] NVARCHAR(100) NULL
)
