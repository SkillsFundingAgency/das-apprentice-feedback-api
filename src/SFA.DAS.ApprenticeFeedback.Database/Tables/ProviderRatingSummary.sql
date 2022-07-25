CREATE TABLE [dbo].[ProviderRatingSummary]

(
    Ukprn BIGINT NOT NULL,
    Rating NVARCHAR(20) NOT NULL,
    RatingCount INT NOT NULL, 
    UpdatedOn DATETIME2
    PRIMARY KEY(Ukprn, Rating) 
)    

GO