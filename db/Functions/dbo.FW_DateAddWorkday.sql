SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

-- =============================================
-- Author:		Yulius
-- Create date: 13 Mei 2015
-- Description:	DateAdd dengan workday
-- =============================================
-- Print dbo.[FW_DateAddWorkday]('2019-11-04 10:00',2,default)
CREATE FUNCTION [dbo].[FW_DateAddWorkday]
(
	-- Add the parameters for the function here
	@tglMulai DATETIME,
	@workday INT,
	@type VARCHAR(20) = 'Standard'
)
RETURNS datetime
AS
BEGIN
	DECLARE @result DATETIME, @tempTglMulai DATETIME
	SELECT @tempTglMulai = dbo.FW_GetDateWithoutTime(@tglMulai)

	IF @workday >= 0
	BEGIN
		SELECT @result = calendardate 
		FROM (
			SELECT DISTINCT calendardate,  
			ROW_NUMBER() OVER (ORDER BY calendardate) - 1 urut 
			FROM dbo.FWCalendar c
			LEFT JOIN dbo.FWWorkday w ON w.calendartype = c.calendartype AND w.workdayyear = c.calendaryear  AND w.workdaydayName = c.calendardayName
			WHERE c.calendartype = @type AND c.calendarholiday = 0	
			AND calendardate >= @tempTglMulai 
		) a WHERE a.urut = @workday
	END
	ELSE
	BEGIN
		SELECT @workday = - @workday
		SELECT @result = calendardate 
		FROM (
			SELECT DISTINCT calendardate,  
			ROW_NUMBER() OVER (ORDER BY calendardate DESC) - 1 urut 
			FROM dbo.FWCalendar c
			LEFT JOIN dbo.FWWorkday w ON w.calendartype = c.calendartype AND w.workdayyear = c.calendaryear  AND w.workdaydayName = c.calendardayName
			WHERE c.calendartype = @type AND c.calendarholiday = 0	
			AND calendardate <= @tempTglMulai 
		) a WHERE a.urut = @workday

	END
	
	-- default = maksimal
	IF @result IS NULL	
	BEGIN
		SELECT TOP 1 @result = calendardate FROM dbo.FWCalendar WHERE calendarholiday = 0 AND calendartype = @type
		ORDER BY calendardate DESC	
		DECLARE @errtext VARCHAR(500) 
		SELECT @errtext = 'Master kalender belum disetting. Tanggal kalender terakhir : ' + FORMAT(@result,'dd-MM-yyyy')
		return cast(@errtext as int);
	END
	SELECT @result = CAST(CONVERT(VARCHAR,@result,102) + ' ' + CONVERT(VARCHAR,@tglMulai,108) AS DATETIME) 
	RETURN @result

END

GO
