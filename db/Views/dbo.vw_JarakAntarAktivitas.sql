SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE VIEW [dbo].[vw_JarakAntarAktivitas]
AS 
SELECT
	a.rowid,
	a.periode,
	a.member_code + ' - ' + c.nama_ljk AS LJK,
	a.member_type_code,
	a.member_code,
	CAST(a.user_id AS REAL) AS user_id,
	b.act_count,
	a.mean_diff,
	a.status_avg_activity,
	SUBSTRING(a.status_avg_activity,0,8) AS status 
FROM dbo.BDA_LogOutliers_Avg a
LEFT OUTER JOIN dbo.BDA_LogOutliers_Act b ON b.periode = a.periode AND b.member_type_code = a.member_type_code AND b.member_code = a.member_code AND b.user_id = a.user_id
LEFT OUTER JOIN dbo.master_ljk c ON a.member_type_code = c.kode_jenis_ljk AND a.member_code = c.kode_ljk
GO
