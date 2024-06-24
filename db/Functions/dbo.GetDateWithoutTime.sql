SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 13 Jan 2009
-- Description:	Mengambil tanggal yang sudah bersih dari hour, minute dan detik
-- =============================================
CREATE FUNCTION [dbo].[GetDateWithoutTime] 
(
	-- Add the parameters for the function here
	@tgl datetime
)
RETURNS datetime
AS
BEGIN
	
	RETURN CAST(FLOOR( CAST( @tgl AS FLOAT ) )AS DATETIME)

END

GO
