SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Ricky
-- Modify date: 04 Aug 2021
-- Description:	Cek apakah suatu user memiliki hak akses modul atau tidak
-- =============================================
--[FW_userPermission] 'admin','AdminRencana'
CREATE PROCEDURE [dbo].[FW_userPermission] 
	@user_id VARCHAR(250) = NULL,
	@mod_kode VARCHAR(500) = NULL,		
	@role_name VARCHAR(200) = NULL
AS
BEGIN
DECLARE @sql NVARCHAR(4000)
SELECT @sql ='        
SELECT UserId, RoleName, ModKode FROM (        
SELECT DISTINCT fur.UserId, rro.RoleName, ModKode = LTRIM(RTRIM(frm.ModKode)) + '' '' + value    
FROM (SELECT * FROM dbo.FWUserRole        
WHERE Stsrc = ''A'''         
IF @user_id IS NOT NULL         
 SELECT @sql = @sql + ' AND UserId = @user_id'        
        
SELECT @sql = @sql + ') fur       
JOIN (SELECT UserId FROM UserMaster WHERE Stsrc = ''A'''        
      
IF @user_id IS NOT NULL         
 SELECT @sql = @sql + ' AND UserId = @user_id'        
        
SELECT @sql = @sql + '      
) mu on fur.UserId = mu.UserId      
JOIN (SELECT * FROM dbo.FWRefRole WHERE Stsrc = ''A'''         
        
IF @role_name IS NOT NULL         
 SELECT @sql = @sql + ' AND RoleName = @role_name'        
        
SELECT @sql = @sql + ')rro ON rro.RoleId = fur.RoleId         
JOIN (SELECT RoleId, ModId, value        
   FROM (SELECT RoleId, ModId,         
    IsView = CAST(CASE WHEN IsView = 1 THEN ''View'' ELSE '''' END AS VARCHAR(10)),         
    IsAdd = CAST(CASE WHEN IsAdd = 1 THEN ''Add'' ELSE '''' END AS VARCHAR(10)),         
    IsEdit = CAST(CASE WHEN IsEdit = 1 THEN ''Edit'' ELSE '''' END AS VARCHAR(10)),         
    IsDelete = CAST(CASE WHEN IsDelete = 1 THEN ''Delete'' ELSE '''' END AS VARCHAR(10)),      
    IsReview = CAST(CASE WHEN IsReview = 1 THEN ''Review'' ELSE '''' END AS VARCHAR(10)),         
    IsApprove = CAST(CASE WHEN IsApprove = 1 THEN ''Approve'' ELSE '''' END AS VARCHAR(10)),         
    IsOpen = CAST(CASE WHEN IsOpen = 1 THEN ''Open'' ELSE '''' END AS VARCHAR(10)),         
    IsPrint = CAST(CASE WHEN IsPrint = 1 THEN ''Print'' ELSE '''' END AS VARCHAR(10)),
	IsExport = CAST(CASE WHEN IsExport = 1 THEN ''Export'' ELSE '''' END AS VARCHAR(10))        
    FROM FWRoleRight WHERE Stsrc = ''A'') A        
   UNPIVOT (value for name in (IsView, IsAdd, IsEdit, IsDelete, IsReview, IsApprove, IsOpen, IsPrint, IsExport)) unpiv        
  WHERE value <> ''''        
   )frr ON frr.RoleId = fur.RoleId        
JOIN (SELECT * FROM dbo.FWRefModul WHERE Stsrc = ''A'')frm ON frm.ModId = frr.ModId         
WHERE fur.Stsrc = ''A''        
) RESULT WHERE 1=1         
'        
    
IF @mod_kode IS NOT NULL         
 SELECT @sql = @sql + ' AND ModKode IN (SELECT split FROM dbo.Split('','',@mod_kode))'        
        
--PRINT @sql        
EXEC sp_executesql @sql,N'@user_id varchar(50), @mod_kode varchar(500), @role_name varchar(200)', @user_id, @mod_kode, @role_name         
         
END 
GO
