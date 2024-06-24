SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


CREATE VIEW [dbo].[vw_PengawasLJK]
AS	
SELECT ISNULL(ROW_NUMBER() OVER ( ORDER BY user_login_id ), 0) AS rownum , pl.user_login_id,
       pl.orgn_name,
       pl.employee_id,
       pl.phone_number,
       pl.group_name,
       pl.active_flag,
       pl.member_type_code,
       pl.member_code,
       pl.created_datetime,
       pl.created_by,
       pl.updated_datetime,
       pl.updated_by,
       pl.p_date,lt.deskripsi_jenis_ljk,l.nama_ljk,pl.sync_date FROM dbo.pengawas_ljk pl
LEFT JOIN dbo.master_ljk_type lt ON lt.kode_jenis_ljk=pl.member_type_code
LEFT JOIN dbo.master_ljk l ON l.kode_jenis_ljk=pl.member_type_code AND l.kode_ljk=pl.member_code
WHERE l.status_aktif='Y' AND lt.status_aktif='Y' AND pl.active_flag='Y'
GO
