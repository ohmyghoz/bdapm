SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

-- =============================================
-- Author:		Yulius
-- Create date: 12 Okt 2008
-- Description:	Mengembalikan datetime dari suatu xml
-- =============================================
CREATE FUNCTION [dbo].[FW_GetDateFromXmlDate]
(
	-- Add the parameters for the function here
	@xmlDate char(25)
)
RETURNS datetime
AS
BEGIN



	declare @tempStr char(19)
	select @tempStr = @xmlDate
	return cast(replace(@tempStr,'T',' ') as datetime)
END

GO
