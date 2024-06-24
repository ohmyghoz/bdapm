SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author	  :	Ricky
-- Modify date: 04 Jan 2022
-- Description:	Get data dari table Alert_Summary sesuai dengan Kode Alert yang dipilih
-- =============================================
CREATE PROCEDURE [dbo].[usp_getGroupedAlertSummary]
	@kode_alert VARCHAR(50) = NULL,
	@kode_alert_kategori NVARCHAR(MAX) = NULL,
	@level_alert VARCHAR(50) = NULL,
	@jenis_ljk NVARCHAR(MAX) = NULL,
	@ljk NVARCHAR(MAX) = NULL,
	@office NVARCHAR(MAX) = NULL,
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
	SET @sql = ''
	IF @kode_alert IS NULL AND @periode_awal IS NULL AND @periode_akhir IS NULL
		BEGIN
			SET @sql = @sql + 'SELECT '''' AS KODE_ALERT, '''' AS NAMA_ALERT, '''' AS TIPE_PERIODE, '''' AS MEMBER_TYPE_CODE, '''' AS JenisLJK, '''' AS MEMBER_CODE, '''' AS LJK, '''' AS KodeKantorCabang, '''' AS KantorCabang, '''' AS UserId, '''' AS UserName, '''' AS Frekuensi, '''' AS Waktu, 0 AS RED, 0 AS YELLOW, 0 AS GREEN, 0 AS GREY '
		END
	ELSE
		BEGIN
			SET @sql = '
			DECLARE @temp_kodeAlert AS TABLE(split VARCHAR(200))
				INSERT INTO @temp_kodeAlert SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @kode_alert_kategori)
			DECLARE @temp_levelAlert AS TABLE(split VARCHAR(200))
				INSERT INTO @temp_levelAlert SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @level_alert)
			DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
				INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
			DECLARE @temp_member AS TABLE(split VARCHAR(200))
				INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk) option (maxrecursion 0)
			DECLARE @temp_office AS TABLE(split VARCHAR(200))
				INSERT INTO @temp_office SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @office)
			SELECT
				al.KODE_ALERT,
				am.NAMA_ALERT,
				al.TIPE_PERIODE,
				al.MEMBER_TYPE_CODE,
				al.MEMBER_TYPE_CODE + '' - '' + mlt.deskripsi_jenis_ljk AS JenisLJK,
				al.MEMBER_CODE,
				al.MEMBER_CODE + '' - '' + ml.nama_ljk AS LJK'
			SET @sql = @sql +
				CASE @kode_alert
				--Tambah jenis alert lain di sini
					WHEN 'UP'
						THEN ', '''' AS KodeKantorCabang, '''' AS KantorCabang, al.DIMENSI2 AS UserId, al.DIMENSI2 + '' - '' + mui.user_name AS UserName, '''' AS Frekuensi, '''' AS Waktu'
					WHEN 'KC'
						THEN ', RIGHT(al.OFFICE_CODE, 3) AS KodeKantorCabang, RIGHT(al.OFFICE_CODE, 3) + '' - '' + mol.kantor_cabang AS KantorCabang, '''' AS UserId, '''' AS UserName, '''' AS Frekuensi, '''' AS Waktu'
					WHEN 'RJP'
						THEN ', '''' AS KodeKantorCabang, '''' AS KantorCabang, '''' AS UserID, '''' AS UserName, al.DIMENSI2 AS Frekuensi, '''' AS Waktu'
					ELSE ', '''' AS KodeKantorCabang, '''' AS KantorCabang, '''' AS UserId, '''' AS UserName, '''' AS Frekuensi, '''' AS Waktu'
				END
			SET @sql = @sql + ', al.RED,
								al.YELLOW,
								al.GREEN,
								al.GREY
								FROM (
									SELECT
										al.KODE_ALERT,
										al.TIPE_PERIODE,
										al.MEMBER_TYPE_CODE,
										al.MEMBER_CODE,'
			SET @sql = @sql + 
				CASE @kode_alert
				--Tambah jenis alert lain di sini
					WHEN 'KC'
						THEN ' al.OFFICE_CODE,'
					WHEN 'RWP'
						THEN ''
					ELSE ' al.DIMENSI2,'
				END
			SET @sql = @sql +
				CASE @kode_alert
				--Tambah jenis alert lain di sini
					WHEN 'UNI'
						THEN ' CAST(SUM(CASE WHEN al.LEVEL_ALERT = ''3'' THEN NILAI2 ELSE 0 END) AS INTEGER) AS RED,
						CAST(SUM(CASE WHEN al.LEVEL_ALERT = ''2'' THEN NILAI2 ELSE 0 END) AS INTEGER) AS YELLOW,
						CAST(SUM(CASE WHEN al.LEVEL_ALERT = ''1'' THEN NILAI2 ELSE 0 END) AS INTEGER) AS GREEN,
						CAST(SUM(CASE WHEN al.LEVEL_ALERT = ''0'' THEN NILAI2 ELSE 0 END) AS INTEGER) AS GREY
						FROM dbo.Alert_Summary al '
					ELSE
					' SUM(CASE WHEN al.LEVEL_ALERT = ''3'' THEN 1 ELSE 0 END) AS RED,
						SUM(CASE WHEN al.LEVEL_ALERT = ''2'' THEN 1 ELSE 0 END) AS YELLOW,
						SUM(CASE WHEN al.LEVEL_ALERT = ''1'' THEN 1 ELSE 0 END) AS GREEN,
						SUM(CASE WHEN al.LEVEL_ALERT = ''0'' THEN 1 ELSE 0 END) AS GREY
						FROM dbo.Alert_Summary al'
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
				SET @sql = @sql + ' AND RIGHT(al.OFFICE_CODE, 3) IN (SELECT split FROM @temp_office)'
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
			SET @sql = @sql + 
				CASE @kode_alert
				--Tambah jenis alert lain di sini
					WHEN 'KC'
						THEN ' GROUP BY al.KODE_ALERT, al.TIPE_PERIODE, al.MEMBER_TYPE_CODE, al.MEMBER_CODE, al.OFFICE_CODE'
					WHEN 'RWP'
						THEN ' GROUP BY al.KODE_ALERT, al.TIPE_PERIODE, al.MEMBER_TYPE_CODE, al.MEMBER_CODE'
					ELSE ' GROUP BY al.KODE_ALERT, al.TIPE_PERIODE, al.MEMBER_TYPE_CODE, al.MEMBER_CODE, al.DIMENSI2'
				END
			SET @sql = @sql + ' HAVING SUM(CASE WHEN al.LEVEL_ALERT = ''3'' OR al.LEVEL_ALERT = ''2'' OR al.LEVEL_ALERT = ''1'' THEN 1 ELSE 0 END) > 0) al'
			SET @sql = @sql + ' LEFT JOIN dbo.Alert_Master am ON al.KODE_ALERT = am.KODE_ALERT
								LEFT JOIN dbo.master_ljk_type mlt ON al.MEMBER_TYPE_CODE = mlt.kode_jenis_ljk
								LEFT JOIN dbo.master_ljk ml ON al.MEMBER_TYPE_CODE = ml.kode_jenis_ljk AND al.MEMBER_CODE = ml.kode_ljk'
			SET @sql = @sql +
				CASE @kode_alert
				--Tambah jenis alert lain di sini
					WHEN 'UP'
						THEN ' LEFT JOIN dbo.master_user_ideb mui ON al.DIMENSI2 = mui.user_id AND al.MEMBER_TYPE_CODE = mui.member_type_code AND al.MEMBER_CODE = mui.member_code'
					WHEN 'KC'
						THEN ' LEFT JOIN dbo.master_office_ljk mol ON RIGHT(al.OFFICE_CODE, 3) = mol.kode_kantor_cabang AND al.MEMBER_TYPE_CODE = mol.kode_jenis_ljk AND al.MEMBER_CODE = mol.kode_ljk'
					ELSE ''
				END
		END
	EXEC sys.sp_executesql @sql, N'@kode_alert VARCHAR(50), @kode_alert_kategori NVARCHAR(MAX), @level_alert VARCHAR(50), @jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @office NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20), @datamart VARCHAR(20), @dimensi1 VARCHAR(200), @dimensi2 VARCHAR(200), @dimensi3 VARCHAR(200), @dimensi4 VARCHAR(200), @dimensi5 VARCHAR(200)', @kode_alert, @kode_alert_kategori, @level_alert, @jenis_ljk, @ljk, @office, @periode_awal, @periode_akhir, @datamart, @dimensi1, @dimensi2, @dimensi3, @dimensi4, @dimensi5
	PRINT @sql
END
    
GO
