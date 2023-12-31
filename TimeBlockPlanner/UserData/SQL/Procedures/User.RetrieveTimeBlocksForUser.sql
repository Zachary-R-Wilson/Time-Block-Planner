﻿CREATE OR ALTER PROCEDURE [User].RetrieveTimeBlocksForUser
   @UserId INT
AS

SELECT UT.TimeBlockId, UT.UserId, UT.[Name], UT.[Description],
   UT.[Date], UT.TimePeriod
FROM [User].TimeBlock UT
WHERE UT.UserId = @UserId
ORDER BY UT.TimePeriod ASC;
GO