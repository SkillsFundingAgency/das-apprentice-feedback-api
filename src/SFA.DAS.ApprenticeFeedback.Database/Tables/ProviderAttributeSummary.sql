CREATE TABLE [dbo].[ProviderAttributeSummary]
(
    Ukprn BIGINT NOT NULL,
    AttributeId INT NOT NULL, 
    Agree INT NOT NULL DEFAULT 0, 
    Disagree INT NOT NULL DEFAULT 0, 
    UpdatedOn Datetime2,
	PRIMARY KEY(Ukprn, AttributeId) 
)    
​
GO