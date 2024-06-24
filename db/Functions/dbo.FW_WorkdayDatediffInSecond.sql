SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


-- =============================================
-- Author:		Yulius
-- Create date: 2 Sept 2014
-- Desc : Lebih efisien untuk jarak yang lebih besar, dengan hasil yang sama
-- =============================================
CREATE FUNCTION [dbo].[FW_WorkdayDatediffInSecond] 
(
	-- Add the parameters for the function here
	@tglMulai DATETIME,
	@tglSampai DATETIME,
	@type VARCHAR(50) = NULL
)
RETURNS INT
AS
BEGIN
	DECLARE @hasil INT
	SELECT @hasil =  SUM (CASE WHEN calc_start > calc_end THEN 0 ELSE DATEDIFF(s,calc_start,calc_end) END) FROM (
	SELECT *,
	CASE WHEN cal_workday_start > @tglMulai THEN cal_workday_start ELSE @tglMulai END AS calc_start,
	CASE WHEN cal_workday_end < @tglSampai THEN cal_workday_end ELSE @tglSampai END AS calc_end
	FROM (
		SELECT calendardate, calendardayName,  c.calendartype,
		CAST(CONVERT(VARCHAR,calendardate,102) + ' ' + CONVERT(VARCHAR,workdaystart,108) AS DATETIME) AS cal_workday_start,
		CAST(CONVERT(VARCHAR,calendardate,102) + ' ' + CONVERT(VARCHAR,workdayend,108) AS DATETIME) AS cal_workday_end
		FROM dbo.FWCalendar c
		LEFT JOIN dbo.FWWorkday w ON c.calendartype = w.calendartype AND c.calendardayName = w.workdaydayName 
		AND c.calendaryear = w.workdayyear AND c.calendartype = w.calendartype
		WHERE calendarholiday = 0
	)a 
	WHERE
	a.calendardate >= dbo.FW_GetDateWithoutTime(@tglMulai)  AND a.calendardate <= dbo.FW_GetDateWithoutTime(@tglSampai) 
	AND calendartype = @type
) b

	RETURN COALESCE(@hasil,0)
END


GO
