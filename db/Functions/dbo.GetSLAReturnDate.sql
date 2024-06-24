SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION [dbo].[GetSLAReturnDate]
(
   -- Add the parameters for the function here
   @originDate AS DATETIME,
   @sla as INT,
   @type AS VARCHAR(50)
)
RETURNS Datetime
AS
BEGIN
DECLARE @returnDate DATETIME
IF @type = 'Working Day'
BEGIN	
	SELECT @returnDate = x.CalendarDate
	FROM
	(SELECT ROW_NUMBER() OVER (ORDER BY CalendarId) Nomor, CalendarDate
	FROM dbo.FWCalendar
	WHERE CalendarHoliday = 0 AND CalendarDate > @originDate) x
	WHERE x.Nomor=@sla
END
ELSE
BEGIN
	SELECT @returnDate = CalendarDate
	FROM
    (SELECT ROW_NUMBER() OVER (ORDER BY CalendarId) Nomor, CalendarDate
	FROM dbo.FWCalendar
	WHERE CalendarDate > @originDate) x
	WHERE x.Nomor=@sla	
END
	SET @returnDate = DATEADD(HOUR, DATEPART(HOUR, @originDate), @returnDate)
	SET @returnDate = DATEADD(MINUTE, DATEPART(MINUTE, @originDate), @returnDate)
	SET @returnDate = DATEADD(SECOND, DATEPART(SECOND, @originDate), @returnDate)
RETURN @returnDate

END
GO
