﻿CREATE TABLE [dbo].[ProviderStarsSummary]
(
    Ukprn BIGINT NOT NULL,
    ReviewCount INT NOT NULL DEFAULT 0,
    Stars INT NOT NULL
    PRIMARY KEY (Ukprn)
)​
GO