SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author	  :	Ricky
-- Modify date: 22 Nov 2021
-- Description:	Get data dari table Alert_Summary sesuai dengan Kode Alert yang dipilih
-- =============================================
CREATE PROCEDURE [dbo].[usp_getAlertSummary]
	@kode_alert VARCHAR(50) = NULL,
	@kode_alert_kategori NVARCHAR(MAX) = NULL,
	@level_alert VARCHAR(50) = NULL,
	@jenis_ljk NVARCHAR(MAX) = NULL,
	@ljk NVARCHAR(MAX) = NULL,
	@office VARCHAR(50) = NULL,
	@periode_awal VARCHAR(20) = NULL,
	@periode_akhir VARCHAR(20) = NULL,
	@datamart VARCHAR(20) = NULL,
	@dimensi1 VARCHAR(200) = NULL,
	@dimensi2 VARCHAR(200) = NULL,
	@dimensi3 VARCHAR(200) = NULL,
	@dimensi4 VARCHAR(200) = NULL,
	@dimensi5 VARCHAR(200) = NULL
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = '
		DECLARE @temp_kodeAlert AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_kodeAlert SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @kode_alert_kategori)
		DECLARE @temp_levelAlert AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_levelAlert SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @level_alert)
		DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
		DECLARE @temp_member AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)
		SELECT
			al.rowid,
			al.KODE_ALERT,
			am.NAMA_ALERT,
			al.TIPE_PERIODE,
			al.MEMBER_TYPE_CODE,
			mlt.deskripsi_jenis_ljk,
			JenisLJK = al.MEMBER_TYPE_CODE + '' - '' + mlt.deskripsi_jenis_ljk,
			al.MEMBER_CODE,
			LJK = al.MEMBER_CODE + '' - '' + ml.nama_ljk,
			al.OFFICE_CODE,
			al.DIMENSI1,
			al.DIMENSI2,
			al.DIMENSI3,
			al.DIMENSI4,
			al.DIMENSI5,
			al.LEVEL_ALERT,
			NILAI1 = COALESCE(al.NILAI1, 0.000000),
			al.NILAI_PEMBANDING1,
			al.NILAI2,
			al.NILAI_PEMBANDING2,'
	SET @sql = @sql +
		CASE @kode_alert
			WHEN 'UP'
				THEN ' '''' AS KantorCabang, al.DIMENSI2 + '' - '' + mui.user_name AS UserName, Average = 0.000000, al.PERIODE'
			WHEN 'KC'
				THEN ' RIGHT(al.OFFICE_CODE, 3) + '' - '' + mol.kantor_cabang AS KantorCabang, '''' AS UserName, Average = 0.000000, al.PERIODE'
			--WHEN 'SB'
			--	THEN ' '''' AS KantorCabang, '''' AS UserName, Average = 
			--	CASE
			--		WHEN (al.NILAI1 = 0.000000 OR al.NILAI1 IS NULL) OR (al.NILAI_PEMBANDING2 = 0.000000 OR al.NILAI_PEMBANDING2 IS NULL)
			--			THEN 0.000000
			--		ELSE al.NILAI1/al.NILAI_PEMBANDING2
			--	END, al.PERIODE'
			--WHEN 'KOM'
			--	THEN
			--		' al.DIMENSI2 + '' - '' + mol.kantor_cabang AS KantorCabang, al.DIMENSI3 + '' - '' + mui.user_name AS UserName, Average = 0.000000'
			WHEN 'RWP'
				THEN ' '''' AS KantorCabang, '''' AS UserName, Average = 0.000000, PERIODE = 
				CASE
					WHEN @kode_alert = ''RWP_5_Menit''
						THEN DATEADD(MINUTE, -1, DATEADD(MINUTE, 5 * CONVERT(DECIMAL(18,6), DIMENSI2), CONVERT(DATETIME, al.PERIODE)))
					WHEN @kode_alert = ''RWP_30_Menit''
						THEN DATEADD(MINUTE, -1, DATEADD(MINUTE, 30 * CONVERT(DECIMAL(18,6), DIMENSI2), CONVERT(DATETIME, al.PERIODE)))
					ELSE DATEADD(MINUTE, -1, DATEADD(HOUR, CONVERT(DECIMAL(18,6), DIMENSI2), CONVERT(DATETIME, al.PERIODE)))
				END'
			ELSE ' '''' AS KantorCabang, '''' AS UserName, Average = 0.000000, al.PERIODE'
		END
	SET @sql = @sql + '
		FROM dbo.Alert_Summary al
		LEFT JOIN dbo.Alert_Master am ON al.KODE_ALERT = am.KODE_ALERT
		LEFT JOIN dbo.master_ljk_type mlt ON al.MEMBER_TYPE_CODE = mlt.kode_jenis_ljk
		LEFT JOIN dbo.master_ljk ml ON al.MEMBER_TYPE_CODE = ml.kode_jenis_ljk AND al.MEMBER_CODE = ml.kode_ljk'
	SET @sql = @sql +
		CASE @kode_alert
			WHEN 'UP'
				THEN ' LEFT JOIN dbo.master_user_ideb mui ON al.DIMENSI2 = mui.user_id AND al.MEMBER_TYPE_CODE = mui.member_type_code AND al.MEMBER_CODE = mui.member_code'
			WHEN 'KC'
				THEN ' LEFT JOIN dbo.master_office_ljk mol ON RIGHT(al.OFFICE_CODE, 3) = mol.kode_kantor_cabang AND al.MEMBER_TYPE_CODE = mol.kode_jenis_ljk AND al.MEMBER_CODE = mol.kode_ljk'
			--WHEN 'KOM'
			--	THEN ' LEFT JOIN dbo.master_user_ideb mui ON al.DIMENSI2 = mui.user_id AND al.MEMBER_TYPE_CODE = mui.member_type_code AND al.MEMBER_CODE = mui.member_code
			--		LEFT JOIN dbo.master_office_ljk mol ON al.DIMENSI2 = mol.kode_kantor_cabang AND al.MEMBER_TYPE_CODE = mol.kode_jenis_ljk AND al.MEMBER_CODE = mol.kode_ljk'
			ELSE ''
		END		
	SET @sql = @sql + ' WHERE al.KODE_ALERT LIKE @kode_alert + ''%'''
	IF @kode_alert_kategori IS NOT NULL
		SET @sql = @sql + ' AND al.KODE_ALERT IN (SELECT split FROM @temp_kodeAlert)'
	IF @level_alert IS NOT NULL
		SET @sql = @sql + ' AND al.LEVEL_ALERT IN (SELECT split FROM @temp_levelAlert)'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND al.MEMBER_TYPE_CODE IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND al.MEMBER_CODE IN (SELECT split FROM @temp_member)'
	IF @office IS NOT NULL
		SET @sql = @sql + ' AND RIGHT(al.OFFICE_CODE, 3) = @office'
	IF @datamart IS NOT NULL
		SET @sql = @sql + ' AND al.TIPE_PERIODE = @datamart'
	IF @periode_awal IS NOT NULL AND @periode_akhir IS NOT NULL
		SET @sql = @sql + ' AND al.PERIODE BETWEEN @periode_awal AND @periode_akhir'
	IF @dimensi1 IS NOT NULL
		SET @sql = @sql + ' AND al.DIMENSI1 IN (@dimensi1)'
	IF @dimensi2 IS NOT NULL
		SET @sql = @sql + ' AND al.DIMENSI2 IN (@dimensi2)'
	IF @dimensi3 IS NOT NULL
		SET @sql = @sql + ' AND al.DIMENSI3 IN (@dimensi3)'
	IF @dimensi4 IS NOT NULL
		SET @sql = @sql + ' AND al.DIMENSI4 IN (@dimensi4)'
	IF @dimensi5 IS NOT NULL
		SET @sql = @sql + ' AND al.DIMENSI5 IN (@dimensi5)'
	SET @sql = @sql + ' ORDER BY al.LEVEL_ALERT DESC, al.PERIODE DESC'
	EXEC sys.sp_executesql @sql, N'@kode_alert VARCHAR(50), @kode_alert_kategori NVARCHAR(MAX), @level_alert VARCHAR(50), @jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @office VARCHAR(50), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20), @datamart VARCHAR(20), @dimensi1 VARCHAR(200), @dimensi2 VARCHAR(200), @dimensi3 VARCHAR(200), @dimensi4 VARCHAR(200), @dimensi5 VARCHAR(200)', @kode_alert, @kode_alert_kategori, @level_alert, @jenis_ljk, @ljk, @office, @periode_awal, @periode_akhir, @datamart, @dimensi1, @dimensi2, @dimensi3, @dimensi4, @dimensi5
	PRINT @sql
END
GO
