SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getLogOutliers]
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
			date = CONVERT(VARCHAR(7), periode, 126),
			status_count_activity,
			normal_user_count = CASE status_count_activity
					WHEN ''Normal''
						THEN COUNT(user_id)
					ELSE NULL
				END,
			normal_act_sum = CASE status_count_activity
					WHEN ''Normal''
						THEN SUM(act_count)
					ELSE NULL
				END,
			outliers_user_count = CASE status_count_activity
					WHEN ''Outliers_Above''
						THEN COUNT(user_id)
					ELSE NULL
				END,
			outliers_act_sum = CASE status_count_activity
					WHEN ''Outliers_Above''
						THEN SUM(act_count)
					ELSE NULL
				END
		FROM dbo.BDA_LogOutliers_Act
		WHERE 1=1'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND member_type_code IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND member_code IN (SELECT split FROM @temp_member)'
	IF @periode_awal IS NOT NULL AND @periode_akhir IS NOT NULL
		SET @sql = @sql + ' AND periode BETWEEN @periode_awal AND @periode_akhir'
	SET @sql = @sql + ' GROUP BY CONVERT(VARCHAR(7), periode, 126), status_count_activity'
	SET @sql = @sql + ' ORDER BY CONVERT(VARCHAR(7), periode, 126)'
	EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20)', @jenis_ljk, @ljk, @periode_awal, @periode_akhir
	PRINT @sql
END
GO
