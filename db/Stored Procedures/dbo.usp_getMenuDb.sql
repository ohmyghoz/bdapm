SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROC [dbo].[usp_getMenuDb]            
@user_id VARCHAR(50),
@role_name VARCHAR(50)
AS            
BEGIN

DECLARE @role_id BIGINT


SELECT @role_id = a.RoleId
FROM dbo.FWUserRole a
    LEFT JOIN dbo.FWRefRole b
        ON b.RoleId = a.RoleId
WHERE a.Stsrc = 'A'
      AND b.Stsrc = 'A'
      AND a.UserId = @user_id
      AND b.RoleName = @role_name
  
SELECT             
 Id = ISNULL(Menu.ModId,0),            
 ParentId = Menu.ParentModId,            
 Url = COALESCE(Menu.ModMenuUrl,''),             
 Title = CASE WHEN COALESCE(ModMenuNama,'') <> '' THEN  ModMenuNama         
  ELSE         
  CASE WHEN Menu.Value = 'View' THEN ''            
   WHEN Menu.Value = 'Add' THEN 'Tambah '             
   WHEN Menu.Value = 'Edit' THEN 'Ubah '             
   WHEN Menu.Value = 'Delete' THEN 'Hapus '             
   WHEN Menu.Value = 'Review' THEN 'Review '             
   WHEN Menu.Value = 'Approve' THEN 'Setujui '             
   WHEN Menu.Value = 'Open' THEN 'Buka '             
   WHEN Menu.Value = 'Print' THEN 'Cetak '            
   ELSE ''            
   END + Menu.ModNama        
 END,            
 Description = Menu.ModCatatan,            
 Roles = CASE WHEN menu.ModIsPublic = 1 THEN '*'            
    WHEN Menu.Value IS NULL THEN             
     CASE WHEN (SELECT COUNT(*) FROM dbo.FWRefModul WHERE Stsrc = 'A' AND ParentModId = Menu.ModId) > 0 THEN '*'                       
     ELSE Menu.ModKode + ' ' + Menu.ModMenuAksi  END            
    ELSE Menu.ModKode + ' ' + COALESCE(Menu.Value,'View')             
    END,            
 IconClass = Menu.ModIconClass,            
 IconText = '',            
 ImageUrl = '',            
 IsSection = '',            
 IsHidden = CASE WHEN ISNULL(ModIsHidden, 0) = 1 THEN 'true'   
    ELSE   
     CASE WHEN Menu.ModMenuIsHidden = 1 OR Menu.ModMenuIsHidden IS null THEN 'true'   
      ELSE 'false'   
     END   
   END,            
 HasAccess = CONVERT(BIT, CASE WHEN Menu.ModIsPublic = 1 THEN 1                        
        WHEN Value IS NULL THEN 0            
        ELSE 1 END),            
 LiClass = '',            
 Urut = Menu.ModUrut,
 Value,Menu.ModTooltip    
FROM             
(            
 SELECT DISTINCT            
 frm.ModId, frm.ParentModId, frm.ModKode, frm.ModNama, frm.ModCatatan, frm.ModUrut, frm.ModIconClass, frm.ModIsPublic, frmm.ModMenuAksi, frmm.ModMenuUrl,             
 frmm.ModMenuIsHidden, temp.Value, frmm.ModMenuNama, frm.ModIsHidden,frm.ModTooltip       
 FROM (SELECT * FROM dbo.FWRefModul WHERE Stsrc ='A') frm            
 LEFT JOIN (SELECT ModId = CASE WHEN ModMenuAksi = 'View' THEN ModId ELSE (-1) * ModMenuId END,            
      ParentModId = CASE WHEN ModMenuAksi = 'View' THEN NULL ELSE ModId END, ModMenuAksi, ModMenuUrl, ModMenuIsHidden, ModMenuNama             
    FROM dbo.FWRefModulMenu WHERE Stsrc = 'A' AND ModMenuAksi = 'VIEW')frmm ON frm.ModId = frmm.ModId             
 LEFT JOIN (            
  SELECT frr.RoleId,             
  ModId = CASE WHEN frr.Value = 'View' THEN frr.ModId ELSE (-1) * frmm.ModMenuId END,             
  ParentModId = CASE WHEN frr.Value = 'View' THEN NULL ELSE frr.ModId END,  frr.Value            
  FROM (            
  select RoleId, ModId, Value            
     from (SELECT RoleId, ModId,             
      IsView = CAST(CASE WHEN IsView = 1 THEN 'View' ELSE '' END AS VARCHAR(10)),             
      IsAdd = CAST(CASE WHEN IsAdd = 1 THEN 'Add' ELSE '' END AS VARCHAR(10)),             
      IsEdit = CAST(CASE WHEN IsEdit = 1 THEN 'Edit' ELSE '' END AS VARCHAR(10)),             
      IsDelete = CAST(CASE WHEN IsDelete = 1 THEN 'Delete' ELSE '' END AS VARCHAR(10)),             
      IsReview = CAST(CASE WHEN IsReview = 1 THEN 'Review' ELSE '' END AS VARCHAR(10)),             
      IsApprove = CAST(CASE WHEN IsApprove = 1 THEN 'Approve' ELSE '' END AS VARCHAR(10)),             
      IsOpen = CAST(CASE WHEN IsOpen = 1 THEN 'Open' ELSE '' END AS VARCHAR(10)),             
      IsPrint = CAST(CASE WHEN IsPrint = 1 THEN 'Print' ELSE '' END AS VARCHAR(10))            
      FROM dbo.FWRoleRight WHERE Stsrc = 'A' AND RoleId=@role_id)A        
     UNPIVOT (Value for name in (IsView, IsAdd, IsEdit, IsDelete, IsReview, IsApprove, IsOpen, IsPrint)) unpiv            
    WHERE Value <> '') frr            
  join (SELECT UserId, RoleId FROM dbo.FWUserRole WHERE Stsrc = 'A' AND UserId = @user_id AND RoleId=@role_id) fur ON frr.RoleId = fur.RoleId            
  JOIN (SELECT * FROM dbo.FWRefModulMenu WHERE Stsrc = 'A')frmm ON frr.ModId = frmm.ModId AND (frr.Value = frmm.ModMenuAksi)            
 ) temp ON (temp.ModId = frm.ModId AND frmm.ModMenuAksi = temp.Value AND temp.ParentModId IS NULL)            
            
            
 UNION            
            
SELECT DISTINCT            
frmm.ModId, frmm.ParentModId, frm.ModKode, frm.ModNama, frm.ModCatatan, frm.ModUrut, frm.ModIconClass, frm.ModIsPublic, frmm.ModMenuAksi, frmm.ModMenuUrl,           
frmm.ModMenuIsHidden, temp.Value, frmm.ModMenuNama, frm.ModIsHidden,frm.ModTooltip
FROM (SELECT * FROM dbo.FWRefModul WHERE Stsrc ='A') frm            
JOIN (SELECT ModId = (-1) * ModMenuId,          
     ParentModId = ModId,
  ModMenuAksi, ModMenuUrl, ModMenuIsHidden, ModMenuNama             
 FROM dbo.FWRefModulMenu WHERE Stsrc = 'A'     
 AND ModMenuId IN (      
  SELECT ModMenuId FROM (      
   SELECT ROW_NUMBER() OVER (Partition BY ModId ORDER BY ModMenuUrl) AS [No], ModMenuId, ModMenuAksi FROM dbo.FWRefModulMenu       
   WHERE Stsrc = 'A'      
   )A       
   WHERE (ModMenuAksi = 'View' AND [no] > 1) OR ModMenuAksi <> 'View'      
  )      
)frmm ON frm.ModId = frmm.ParentModId             
LEFT JOIN (            
 SELECT frr.RoleId,             
 ModId = (-1) * frmm.ModMenuId,     
 ParentModId = frr.ModId,     
 frr.Value            
 FROM (            
 select RoleId, ModId, Value            
    from (SELECT RoleId, ModId,             
     IsView = CAST(CASE WHEN IsView = 1 THEN 'View' ELSE '' END AS VARCHAR(10)),             
     IsAdd = CAST(CASE WHEN IsAdd = 1 THEN 'Add' ELSE '' END AS VARCHAR(10)),             
     IsEdit = CAST(CASE WHEN IsEdit = 1 THEN 'Edit' ELSE '' END AS VARCHAR(10)),             
     IsDelete = CAST(CASE WHEN IsDelete = 1 THEN 'Delete' ELSE '' END AS VARCHAR(10)),             
     IsReview = CAST(CASE WHEN IsReview = 1 THEN 'Review' ELSE '' END AS VARCHAR(10)),             
     IsApprove = CAST(CASE WHEN IsApprove = 1 THEN 'Approve' ELSE '' END AS VARCHAR(10)),             
     IsOpen = CAST(CASE WHEN IsOpen = 1 THEN 'Open' ELSE '' END AS VARCHAR(10)),             
     IsPrint = CAST(CASE WHEN IsPrint = 1 THEN 'Print' ELSE '' END AS VARCHAR(10))            
     FROM dbo.FWRoleRight WHERE Stsrc = 'A' AND RoleId=@role_id)A            
    UNPIVOT (Value for name in (IsView, IsAdd, IsEdit, IsDelete, IsReview, IsApprove, IsOpen, IsPrint)) unpiv            
   WHERE Value <> '') frr            
 join (SELECT UserId, RoleId FROM dbo.FWUserRole WHERE Stsrc = 'A' AND UserID = @user_id AND RoleId=@role_id) fur ON frr.RoleId = fur.RoleId            
 JOIN (SELECT * FROM dbo.FWRefModulMenu WHERE Stsrc = 'A')frmm ON frr.ModId = frmm.ModId AND (frr.Value = frmm.ModMenuAksi)            
) temp ON (temp.ParentModId = frm.ModId AND frmm.ModMenuAksi = temp.Value)             
)Menu
ORDER BY ParentId ASC, Urut ASC, Menu.ModMenuUrl DESC            
             
END 
GO
