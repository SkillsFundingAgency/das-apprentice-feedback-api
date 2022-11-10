DECLARE @TableName VARCHAR(500)
SET @TableName='ApprenticeExitSurvey_' + REPLACE(REPLACE(REPLACE((CONVERT(VARCHAR(19),GETDATE(),120)), '-', ''), ' ', ''), ':', '')
DECLARE @Sql NVARCHAR(500);
SET @Sql = N'SELECT * INTO ' + QUOTENAME(@TableName) + ' FROM ApprenticeExitSurvey';
EXEC sp_executesql @Sql;
