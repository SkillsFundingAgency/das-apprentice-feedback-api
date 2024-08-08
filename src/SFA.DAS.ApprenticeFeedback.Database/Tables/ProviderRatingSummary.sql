CREATE TABLE [dbo].[ProviderRatingSummary]

(
    Ukprn BIGINT NOT NULL,
    Rating NVARCHAR(20) NOT NULL,
    RatingCount INT NOT NULL, 
    UpdatedOn DATETIME2,
    TimePeriod NVARCHAR(50) NOT NULL DEFAULT 'All',
    PRIMARY KEY(Ukprn, Rating, TimePeriod) 
)    

GO