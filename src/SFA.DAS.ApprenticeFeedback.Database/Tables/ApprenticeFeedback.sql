CREATE TABLE [dbo].[ApprenticeFeedback]
(
	[FeedbackId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Ukprn] int NOT NULL FOREIGN KEY REFERENCES [dbo].[Providers](Ukprn),
    [StandardUId] NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES [dbo].[Standards](StandardUId), 
    [IsActive] BIT NULL
)
