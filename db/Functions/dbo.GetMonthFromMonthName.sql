SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetMonthFromMonthName]
(
	@monthname VARCHAR(50)
)
RETURNS VARCHAR(2)
AS
BEGIN
	DECLARE @final VARCHAR(2)
	SELECT @final = CASE 
	WHEN @monthname='Januari' THEN '01'
	WHEN @monthname='Februari' THEN '02'
	WHEN @monthname='Maret' THEN '03'
	WHEN @monthname='April' THEN '04'
	WHEN @monthname='Mei' THEN '05'
	WHEN @monthname='Juni' THEN '06'
	WHEN @monthname='Juli' THEN '07'
	WHEN @monthname='Agustus' THEN '08'
	WHEN @monthname='September' THEN '09'
	WHEN @monthname='Oktober' THEN '10'
	WHEN @monthname='November' THEN '11'
	WHEN @monthname='Desember' THEN '12'
	END

	-- Return the result of the function
	RETURN @final

END
GO
