﻿CREATE TABLE [dbo].[ProviderStarsSummary]
(
    Ukprn BIGINT NOT NULL,
    ReviewCount INT NOT NULL DEFAULT 0,
    Stars INT NOT NULL,
    TimePeriod NVARCHAR(50) NOT NULL DEFAULT 'All',
    PRIMARY KEY (Ukprn)
)​
GO