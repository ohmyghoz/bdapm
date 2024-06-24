SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getLogPerHour]
	@jenis_ljk NVARCHAR(MAX) = NULL,
	@ljk NVARCHAR(MAX) = NULL,
	@periode VARCHAR(20) = NULL
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = '
		DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
		DECLARE @temp_member AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)
		SELECT
			periode,
			hour,
			user_sum = SUM(cnt_user_id),
			act_sum = SUM(cnt_act)
		FROM dbo.BDA_Log_PerHour
		WHERE 1=1'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND member_type_code IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND member_code IN (SELECT split FROM @temp_member)'
	IF @periode IS NOT NULL
		SET @sql = @sql + ' AND periode = @periode'
	SET @sql = @sql + ' GROUP BY periode, hour'
	SET @sql = @sql + ' ORDER BY hour'
	EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode VARCHAR(20)', @jenis_ljk, @ljk, @periode
	PRINT @sql
END
GO
