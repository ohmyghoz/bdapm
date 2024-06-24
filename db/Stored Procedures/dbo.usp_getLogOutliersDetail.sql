SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getLogOutliersDetail]
	@jenis_ljk NVARCHAR(MAX) = NULL,
	@ljk NVARCHAR(MAX) = NULL,
	@periode_awal VARCHAR(20) = NULL,
	@periode_akhir VARCHAR(20) = NULL,
	@status VARCHAR(50) = NULL
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = '
		DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
		DECLARE @temp_member AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)
		--DECLARE @temp_status AS TABLE(split VARCHAR(200))
		--	  INSERT INTO @temp_status SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @status)
		SELECT
			lo.rowid,
			lo.periode,
			--LJK = CONCAT(lo.member_code, '' - '', ml.nama_ljk),
			LJK = lo.member_code + '' - '' + ml.nama_ljk,
			CAST(lo.user_id as REAL) as user_id,
			lo.act_count,
			status = LEFT(lo.status_count_activity, 8)
		FROM dbo.BDA_LogOutliers_Act lo
		LEFT JOIN dbo.master_ljk ml ON lo.member_type_code = ml.kode_jenis_ljk AND lo.member_code = ml.kode_ljk
		WHERE 1=1'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND lo.member_type_code IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND lo.member_code IN (SELECT split FROM @temp_member)'
	IF @status IS NOT NULL
		BEGIN
			DECLARE @temp_status2 VARCHAR(2000)
			SELECT @temp_status2 = COALESCE(@temp_status2 + ' OR ' + temp.status, temp.status) FROM (SELECT 'lo.status_count_activity LIKE ''%' + LTRIM(RTRIM(split)) + '%''' AS status FROM dbo.Split(', ', @status)) temp
			SET @sql = @sql + ' AND ('
			SET @sql = @sql + @temp_status2
			SET @sql = @sql + ')'
		END
	IF @periode_awal IS NOT NULL AND @periode_akhir IS NOT NULL
		SET @sql = @sql + ' AND lo.periode BETWEEN @periode_awal AND @periode_akhir'
	SET @sql = @sql + ' ORDER BY lo.periode desc'
	EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20), @status VARCHAR(50)', @jenis_ljk, @ljk, @periode_awal, @periode_akhir, @status
	PRINT @sql
END
GO
