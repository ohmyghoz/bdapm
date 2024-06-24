SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

-- =============================================
-- Author:		Yulius
-- Create date: 13 Mei 2015
-- Description:	DateAdd dengan worksecond
-- =============================================
CREATE FUNCTION [dbo].[FW_DateAddWorksecond]
(
	-- Add the parameters for the function here
	@tglMulai DATETIME,
	@worksecond INT,
	@type VARCHAR(20) = 'Standard'
)
RETURNS datetime
AS
BEGIN

	--DECLARE @runningWorkSecond BIGINT = 0
	DECLARE @tempTable TABLE(
	cal_workday_start DATETIME,
	cal_workday_end DATETIME
	)
	INSERT INTO @tempTable ( cal_workday_start, cal_workday_end )
	SELECT a.cal_workday_start, a.cal_workday_end
	FROM (
		SELECT c.calendardate, 
		CAST(CONVERT(VARCHAR,calendardate,102) + ' ' + CONVERT(VARCHAR,workdaystart,108) AS DATETIME) AS cal_workday_start,
		CAST(CONVERT(VARCHAR,calendardate,102) + ' ' + CONVERT(VARCHAR,workdayend,108) AS DATETIME) AS cal_workday_end
		FROM dbo.FWCalendar c
		LEFT JOIN dbo.FWWorkday w ON w.calendartype = c.calendartype AND w.workdayyear = c.calendaryear  AND w.workdaydayName = c.calendardayName
		WHERE c.calendartype = @type AND c.calendarholiday = 0
	) a WHERE a.cal_workday_end >= @tglMulai

	DECLARE @result DATETIME
	DECLARE @start DATETIME, @end DATETIME, @row_worksecond INT

	DECLARE myCursor CURSOR FOR
	SELECT * FROM @tempTable ORDER BY cal_workday_start
	OPEN myCursor
	FETCH NEXT FROM myCursor INTO @start,@end
	WHILE @@FETCH_STATUS = 0 AND @result IS NULL
	BEGIN
		
		IF @start < @tglMulai 
			SELECT @start = @tglMulai
		
		SELECT @row_worksecond = DATEDIFF(SECOND,@start,@end)
		
		IF @worksecond <= @row_worksecond
			SELECT @result = DATEADD(SECOND,@worksecond, @start)
		
		SELECT @worksecond = @worksecond - @row_worksecond
		
	FETCH NEXT FROM myCursor INTO @start,@end
	END
	CLOSE myCursor
	DEALLOCATE myCursor

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
