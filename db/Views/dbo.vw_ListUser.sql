SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE VIEW	[dbo].[vw_ListUser]
AS 
SELECT um.UserId,
       um.UserNama,
       um.UserAlamat,
       um.UserTelp,
       um.UserEmail,
       um.user_is_notifredalert,
       um.user_is_notifyellowalert ,
	   STUFF((SELECT ',' + rr.RoleName
                     FROM	dbo.FWUserRole ur
					 LEFT JOIN dbo.FWRefRole rr ON rr.RoleId = ur.RoleId
                     WHERE  ur.UserId=um.UserId AND ur.Stsrc='A' AND rr.Stsrc='A'
                     FOR XML PATH('')), 1, 1, '') AS RoleName
FROM dbo.UserMaster um
WHERE um.Stsrc='A'
GO
