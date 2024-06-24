SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


CREATE VIEW [dbo].[vw_Modul] AS 
SELECT  ISNULL(CAST(( ROW_NUMBER() OVER( ORDER BY Result.ParentModId, ModId )) AS INT),0) AS ID,     
 ModId, ParentModId, ModMenuId, ModKodeTeks, ModKode, ModNama, ModUrut, ModKey, ModMenuUrl, ModMenuAksi, Stsrc, ModIsPublic, ModMenuIsHidden    
FROM     
(    
 SELECT ModId, ParentModId, ModMenuId = NULL, ModKodeTeks = ModKode, ModKode, ModNama, ModUrut,     
  ModKey = 'mod_' + CAST(ModId AS VARCHAR(10)), ModMenuUrl = NULL, ModMenuAksi = NULL, Stsrc,    
  ModIsPublic = CASE WHEN ModIsPublic = 1 THEN 'Ya' ELSE 'Tidak' END, ModMenuIsHidden = 'N/A'
  FROM dbo.FWRefModul WHERE Stsrc = 'A'    
 UNION    
 SELECT ModId = (-1) * ModMenuId, ParentModId = frm.ModId, ModMenuId, ModKodeTeks = frmm.ModMenuAksi, mod_kode = frm.ModKode,     
  ModNama = CASE WHEN COALESCE(ModMenuNama,'') <> '' THEN  ModMenuNama   
  ELSE   
   CASE WHEN ModMenuAksi = 'View' THEN ''    
   WHEN ModMenuAksi = 'Add' THEN 'Tambah '     
   WHEN ModMenuAksi = 'Edit' THEN 'Ubah '     
   WHEN ModMenuAksi = 'Delete' THEN 'Hapus '     
   WHEN ModMenuAksi = 'Review' THEN 'Review '     
   WHEN ModMenuAksi = 'Approve' THEN 'Setujui '     
   WHEN ModMenuAksi = 'Open' THEN 'Buka '     
   WHEN ModMenuAksi = 'Print' THEN 'Cetak '    
   ELSE ''    
   END + frm.ModNama  
  END, ModUrut = 0,     
  ModKey = 'menu_' + CAST(frmm.ModMenuId AS VARCHAR(10)), frmm.ModMenuUrl, ModMenuAksi, frmm.Stsrc,     
  ModIsPublic = 'N/A', ModMenuIsHidden = CASE WHEN ModMenuIsHidden = 1 THEN 'Ya' ELSE 'Tidak' END
  FROM dbo.FWRefModulMenu frmm    
  JOIN dbo.FWRefModul frm ON frm.ModId = frmm.ModId    
  WHERE frm.Stsrc = 'A' AND frmm.Stsrc = 'A'    
)Result    
    
GO
