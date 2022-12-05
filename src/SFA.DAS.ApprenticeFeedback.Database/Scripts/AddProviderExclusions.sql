﻿
CREATE TABLE #TempExclusions
(
	[Ukprn] BIGINT,
	[CreatedOn] DATETIME2
)

INSERT INTO #TempExclusions VALUES 
(10040329, '2022-12-02'),
(10006472, '2022-12-02')

MERGE Exclusions TARGET
USING #TempExclusions SOURCE
ON TARGET.Ukprn=SOURCE.Ukprn
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Ukprn, CreatedOn)
VALUES (SOURCE.Ukprn,SOURCE.CreatedOn);

DROP TABLE #TempExclusions