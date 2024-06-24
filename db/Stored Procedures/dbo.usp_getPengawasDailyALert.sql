SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


CREATE PROCEDURE [dbo].[usp_getPengawasDailyALert]
@periode DATETIME,
@tipePeriode VARCHAR(50)
AS
SELECT DISTINCT pl.user_login_id FROM dbo.Alert_Summary als
LEFT JOIN dbo.pengawas_ljk pl ON pl.member_code = als.MEMBER_CODE AND pl.member_type_code = als.MEMBER_TYPE_CODE AND als.LEVEL_ALERT IN ('3','2')
WHERE PERIODE=dbo.GetDateWithoutTime(@periode) AND pl.user_login_id IS NOT NULL AND pl.active_flag='Y' AND als.TIPE_PERIODE=@tipePeriode
GO
