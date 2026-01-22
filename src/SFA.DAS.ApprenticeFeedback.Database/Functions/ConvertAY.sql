CREATE FUNCTION [dbo].[ConvertAY]
-- returns the AY format for a date, as AY2324
(
    @indate datetime
)
RETURNS varchar(6)
BEGIN
RETURN  'AY'+RIGHT(YEAR(DATEADD(month,-7,@indate)),2)+RIGHT(YEAR(DATEADD(month,5,@indate)),2);
END;
GO
