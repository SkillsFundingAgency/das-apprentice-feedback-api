﻿CREATE TABLE [dbo].[Exclusions]
(
	[Ukprn] BIGINT NOT NULL PRIMARY KEY,
	[CreatedOn] DATETIME2 NOT NULL DEFAULT GETDATE()
)
