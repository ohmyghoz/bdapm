SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 13 Nov 2008
-- Description:	Mengambil setting dari Ref_Setting
-- =============================================
CREATE FUNCTION [dbo].[GetSetting]
(
	-- Add the parameters for the function here
	@set_name varchar(250)
)
RETURNS varchar(250)
AS
BEGIN
	declare @set_value varchar(250)
	select @set_value = set_value from FW_Ref_Setting where set_name = @set_name
	return @set_value
END
GO
