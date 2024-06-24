SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getListUserSync]
AS 
SELECT DISTINCT user_login_id FROM dbo.pengawas_ljk WHERE active_flag='Y' AND user_login_id NOT IN (
--SELECT UserId FROM dbo.UserMaster WHERE UserMainRole='PengawasLJK' AND Stsrc='A'
SELECT A.UserId FROM dbo.FWUserRole A
LEFT JOIN dbo.FWRefRole b ON b.RoleId = A.RoleId
WHERE a.Stsrc='A' AND b.Stsrc='A' AND b.RoleName='PengawasLJK'
)
GO
