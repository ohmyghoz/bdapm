SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getListUserPengawasForDelete]
AS 
SELECT a.UserId FROM (
SELECT DISTINCT um.UserId,rr.RoleName FROM dbo.UserMaster um 
LEFT JOIN dbo.FWUserRole ur ON ur.UserId = um.UserId
LEFT JOIN dbo.FWRefRole rr ON rr.RoleId = ur.RoleId
WHERE um.Stsrc='A' AND ur.Stsrc='A'  AND rr.Stsrc='A' AND rr.RoleName='PengawasLJK'
) a
WHERE a.UserId NOT IN (SELECT DISTINCT user_login_id FROM dbo.pengawas_ljk WHERE active_flag='Y')
GO
