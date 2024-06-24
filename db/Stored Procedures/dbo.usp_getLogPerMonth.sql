SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getLogPerMonth]
	@jenis_ljk NVARCHAR(MAX) = NULL,
	@ljk NVARCHAR(MAX) = NULL,
	@periode_awal VARCHAR(20) = NULL,
	@periode_akhir VARCHAR(20) = NULL
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = '
		DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
		DECLARE @temp_member AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)
		SELECT
			lpm.periode,
			JenisLJK = lpm.member_type_code + '' - '' + mlt.deskripsi_jenis_ljk,
			LJK = lpm.member_code + '' - '' + ml.nama_ljk,
			CAST(lpm.user_id as REAL) as user_id,
			lpm.max_act_count,
			lpm.min_act_count,
			CAST(lpm.avg_act_count AS DECIMAL(34,2)) as avg_act_count,
			CAST((CAST(lpm.max_act_count AS DECIMAL(34,2))/CAST(lpm.min_act_count AS DECIMAL(34,2))) AS DECIMAL(34,2)) AS max_to_min,
			CAST((CAST(lpm.max_act_count AS DECIMAL(34,2))/CAST(lpm.avg_act_count AS DECIMAL(34,2))) AS DECIMAL(34,2)) AS max_to_avg
		FROM dbo.BDA_Log_PerMonth lpm
		LEFT JOIN dbo.master_ljk_type mlt ON lpm.member_type_code = mlt.kode_jenis_ljk
		LEFT JOIN dbo.master_ljk ml ON lpm.member_type_code = ml.kode_jenis_ljk AND lpm.member_code = ml.kode_ljk
		WHERE 1=1'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND lpm.member_type_code IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND lpm.member_code IN (SELECT split FROM @temp_member)'
	IF @periode_awal IS NOT NULL AND @periode_akhir IS NOT NULL
		SET @sql = @sql + ' AND lpm.periode BETWEEN @periode_awal AND @periode_akhir'
	SET @sql = @sql + ' ORDER BY lpm.periode DESC'
	EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20)', @jenis_ljk, @ljk, @periode_awal, @periode_akhir
	PRINT @sql
END
GO
