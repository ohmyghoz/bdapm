SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getLogOutliersAvg]
	@jenis_ljk NVARCHAR(MAX) = NULL,
	@ljk NVARCHAR(MAX) = NULL,
	@periode_awal VARCHAR(20) = NULL,
	@periode_akhir VARCHAR(20) = NULL,
	@status VARCHAR(50) = NULL
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = ''
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' DECLARE @temp_member AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)'
	SET @sql = @sql + '
		SELECT
			avg.rowid,
			avg.periode,
			LJK = avg.member_code + '' - '' + ml.nama_ljk,
			CAST(avg.user_id as REAL) as user_id,
			act.act_count,
			avg.mean_diff,
			status = LEFT(avg.status_avg_activity, 8)
		FROM dbo.BDA_LogOutliers_Avg avg
		LEFT OUTER JOIN dbo.BDA_LogOutliers_Act act ON avg.periode = act.periode AND avg.member_type_code = act.member_type_code AND avg.member_code = act.member_code AND avg.user_id = act.user_id
		LEFT OUTER JOIN dbo.master_ljk ml ON avg.member_type_code = ml.kode_jenis_ljk AND avg.member_code = ml.kode_ljk
		WHERE 1=1'
	IF @periode_awal IS NOT NULL AND @periode_akhir IS NOT NULL
		SET @sql = @sql + ' AND avg.periode BETWEEN @periode_awal AND @periode_akhir'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND avg.member_type_code IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND avg.member_code IN (SELECT split FROM @temp_member)'
	IF @status IS NOT NULL
		BEGIN
			DECLARE @temp_status2 VARCHAR(2000)
			SELECT @temp_status2 = COALESCE(@temp_status2 + ' OR ' + temp.status, temp.status) FROM (SELECT 'avg.status_avg_activity LIKE ''%' + LTRIM(RTRIM(split)) + '%''' AS status FROM dbo.Split(', ', @status)) temp
			SET @sql = @sql + ' AND ('
			SET @sql = @sql + @temp_status2
			SET @sql = @sql + ')'
		END
	SET @sql = @sql + ' ORDER BY avg.periode desc'
	EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20), @status VARCHAR(50)', @jenis_ljk, @ljk, @periode_awal, @periode_akhir, @status
	PRINT @sql
END
GO
