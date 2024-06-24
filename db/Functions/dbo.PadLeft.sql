SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 24 Juni 2010
-- Description:	Kode Estimate
-- =============================================
CREATE FUNCTION [dbo].[PadLeft]
(
	@str VARCHAR(50), 
	@padChar CHAR(1),
	@len INT 
)
RETURNS VARCHAR(50)
AS
BEGIN
	DECLARE @ret VARCHAR(50)
	SELECT @ret = REPLICATE(@padChar,@len-LEN(@str)) + @str
	
	RETURN @ret
	
END
GO
