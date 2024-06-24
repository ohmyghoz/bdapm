SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getHmlMcdfa]
	@tipe_dashboard VARCHAR(50) = NULL,
	@kode_report VARCHAR(50) = NULL,
	@jenis_ljk NVARCHAR(MAX) = NULL,
	@ljk NVARCHAR(MAX) = NULL,
	@hml VARCHAR(20) = NULL,
	@mcdfa VARCHAR(20) = NULL,
	@periode_awal VARCHAR(20) = NULL,
	@periode_akhir VARCHAR(20) = NULL,
	@dimensi1 VARCHAR(200) = NULL
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = '
		DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
		DECLARE @temp_member AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)
		DECLARE @temp_hml AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_hml SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @hml)
		DECLARE @temp_mcdfa AS TABLE(split VARCHAR(200))
			INSERT INTO @temp_mcdfa SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @mcdfa)
		SELECT'
	SET @sql = @sql +
		CASE @tipe_dashboard
		--hml/mcdfa/all
			WHEN 'hml'
				THEN ' mcdfa = '''', hml = RIGHT(hm.hml_pareto, 1),'
			WHEN 'mcdfa'
				THEN ' hm.mcdfa, hml ='''','
			ELSE ' hm.mcdfa, hml = RIGHT(hm.hml_pareto, 1),'
		END
	SET @sql = @sql + '
			outstanding = SUM(sum_outstanding),
			credit_limit = SUM(sum_credit_limit),
			debitur_count = SUM(sum_qty_distinct)
		FROM dbo.BDA_HML_MCDFA hm'
	--d01 = perorangan
	--d02 = badan usaha
	SET @sql = @sql + ' WHERE hm.kode_report LIKE ''%'' + @kode_report'
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + ' AND hm.member_type_code IN (SELECT split FROM @temp_memberType)'
	IF @ljk IS NOT NULL
		SET @sql = @sql + ' AND hm.member_code IN (SELECT split FROM @temp_member)'
	IF @periode_awal IS NOT NULL AND @periode_akhir IS NOT NULL
		SET @sql = @sql + ' AND hm.periode BETWEEN @periode_awal AND @periode_akhir'
	IF @hml IS NOT NULL
		SET @sql = @sql + ' AND hm.hml_pareto IN (SELECT split FROM @temp_hml)'
	IF @mcdfa IS NOT NULL
		SET @sql = @sql + ' AND hm.mcdfa IN (SELECT split FROM @temp_mcdfa)'
	IF @dimensi1 IS NOT NULL
		SET @sql = @sql + ' AND hm.dimensi1 = @dimensi1'
	SET @sql = @sql + ' GROUP BY' +
		CASE @tipe_dashboard
		--hml/mcdfa/all
			WHEN 'hml'
				THEN ' RIGHT(hm.hml_pareto, 1)'
			WHEN 'mcdfa'
				THEN ' hm.mcdfa'
			ELSE ' hm.mcdfa, hm.hml_pareto'
		END
	EXEC sys.sp_executesql @sql, N'@tipe_dashboard VARCHAR(50), @kode_report VARCHAR(50), @jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @hml VARCHAR(20), @mcdfa VARCHAR(20), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20), @dimensi1 VARCHAR(200)', @tipe_dashboard, @kode_report, @jenis_ljk, @ljk, @hml, @mcdfa, @periode_awal, @periode_akhir, @dimensi1
	PRINT @sql
END
GO
