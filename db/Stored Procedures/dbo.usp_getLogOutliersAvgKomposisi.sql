SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getLogOutliersAvgKomposisi]
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
			user_count = COUNT (DISTINCT user_id),
			mean_diff_avg = AVG(mean_diff),
			status = LEFT(status_avg_activity, 8)
		FROM dbo.BDA_LogOutliers_Avg
		WHERE 1=1'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND member_type_code IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND member_code IN (SELECT split FROM @temp_member)'
	IF @periode_awal IS NOT NULL AND @periode_akhir IS NOT NULL
		SET @sql = @sql + ' AND periode BETWEEN @periode_awal AND @periode_akhir'
	SET @sql = @sql + ' GROUP BY status_avg_activity'
	EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20)', @jenis_ljk, @ljk, @periode_awal, @periode_akhir
	PRINT @sql
END
GO
