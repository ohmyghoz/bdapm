SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 16 Juni 2009
-- Description:	Print String yang sangat panjang
-- =============================================
CREATE PROCEDURE [dbo].[PrintLongString]
@str VARCHAR(MAX) 
AS
BEGIN

DECLARE @count INT, @i INT
SELECT @count = CAST((LEN(@str) / 4000) AS INT) + 1

SELECT @i = 0
WHILE @count > @i
BEGIN
	PRINT SUBSTRING(@str,@i * 4000,4000)
	SET @i = @i + 1

END 

End
GO
