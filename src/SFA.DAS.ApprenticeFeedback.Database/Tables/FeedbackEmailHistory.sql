CREATE TABLE [dbo].[FeedbackEmailHistory]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ApprenticeId] NCHAR(10) NULL, 
    [SentDate] TIMESTAMP NOT NULL, 
    [DeliveryNotification] NCHAR(10) NULL
)
