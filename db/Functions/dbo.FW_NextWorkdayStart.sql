SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

-- =============================================
-- Author:		Yulius
-- Create date: 13 Mei 2015
-- Description:	DateAdd dengan workday
-- =============================================
-- Print dbo.[FW_NextWorkdayStart]('2019-11-04 10:12:00',default)
CREATE FUNCTION [dbo].[FW_NextWorkdayStart]
(
	-- Add the parameters for the function here
	@tglMulai DATETIME,
	@type VARCHAR(20) = 'Standard'
)
RETURNS datetime
AS
BEGIN
	DECLARE @result DATETIME, @tempTglMulai DATETIME
	SELECT @tempTglMulai = dbo.FW_GetDateWithoutTime(@tglMulai)

	SELECT TOP 1 @result = CAST(FORMAT(c.calendardate,'yyyy-MM-dd') + ' 00:00:00' AS DATETIME) 	
	FROM dbo.FWCalendar c
	INNER JOIN dbo.FWWorkday w ON w.calendartype = c.calendartype AND w.workdayyear = c.calendaryear  AND w.workdaydayName = c.calendardayName
	WHERE c.calendartype = @type AND c.calendarholiday = 0		
	AND calendardate > @tempTglMulai 
	ORDER BY c.calendardate



	-- default = maksimal
	IF @result IS NULL	
	BEGIN
		SELECT TOP 1 @result = calendardate FROM dbo.FWCalendar WHERE calendarholiday = 0 AND calendartype = @type
		ORDER BY calendardate DESC	
		DECLARE @errtext VARCHAR(500) 
		SELECT @errtext = 'Master kalender belum disetting. Tanggal kalender terakhir : ' + FORMAT(@result,'dd-MM-yyyy')
		return cast(@errtext as int);
	END
	
	RETURN @result

END

GO
