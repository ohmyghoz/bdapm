SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

-- =============================================
-- Author:		Yulius
-- Create date: 26 Maret 2014
-- =============================================
-- print dbo.[FW_WorkdayDatediffInDay]('2019-07-01','2019-05-01','Standard')
CREATE FUNCTION [dbo].[FW_WorkdayDatediffInDay] 
(
	-- Add the parameters for the function here
	@tglMulai DATETIME,
	@tglSampai DATETIME,
	@type VARCHAR(50) = NULL
)
RETURNS INT
AS
BEGIN
	--DECLARE	@tglMulai DATETIME,	@tglSampai DATETIME,@type VARCHAR(50) = 'Standard'
	--SELECT @tglMulai = '2014-03-25 10:00:00', @tglSampai = '2014-03-26 12:30:00'
	DECLARE @workdayDiff AS INTEGER

	DECLARE @isReverse BIT = 0;
	IF @tglMulai > @tglSampai
	BEGIN
		SET @isReverse = 1
		DECLARE @temp DATETIME
		SET @temp = @tglMulai
		SET @tglMulai = @tglSampai
		SET @tglSampai = @temp
	END

	SELECT @workdayDiff = COUNT(DISTINCT calendardate) FROM (
		SELECT calendardate, calendardayName, calendarholiday, c.calendartype,
		CAST(CONVERT(VARCHAR,calendardate,102) + ' ' + CONVERT(VARCHAR,workdaystart,108) AS DATETIME) AS cal_workday_start,
		CAST(CONVERT(VARCHAR,calendardate,102) + ' ' + CONVERT(VARCHAR,workdayend,108) AS DATETIME) AS cal_workday_end
		FROM dbo.FWCalendar c
		LEFT JOIN dbo.FWWorkday w ON c.calendartype = w.calendartype AND c.calendardayName = w.workdaydayName 
		AND c.calendaryear = w.workdayyear AND c.calendartype = w.calendartype		
		AND calendarholiday = 0
	) a
	WHERE 
	cal_workday_start < @tglSampai AND cal_workday_end > @tglMulai 
	AND calendartype = @type

	IF @isReverse = 1
		SET @workdayDiff = @workdayDiff * -1


	RETURN @workdayDiff
END

--
GO
