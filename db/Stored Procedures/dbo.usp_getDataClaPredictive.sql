SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getDataClaPredictive]
@periode_awal VARCHAR(20),
@periode_akhir VARCHAR(20),
@jenis_ljk NVARCHAR(MAX),
@ljk NVARCHAR(MAX),
@jenisDebitur VARCHAR(200),
@statusCollectability VARCHAR(100),
@dugaanCollectability VARCHAR(100)
AS 
DECLARE @sql NVARCHAR(MAX)
SET @sql = ' '
IF @periode_awal IS NULL AND @periode_akhir IS NULL
BEGIN
SET @sql=@sql+'SELECT rowid,
       kode_report,
       periode,
       member_type_code,
       member_code,
       status,
       collectibility_type_code,
	   dugaan_collectability,
       sum_outstanding,
       cnt_row,
       cnt_distinct_cif,
       cnt_acc,
       min_overdue_days,
       max_overdue_days,'''' AS deskripsi_jenis_ljk FROM dbo.BDA_F01_Predictive WHERE rowid=0'
END 
ELSE	
BEGIN
SET @sql=@sql+'
	DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
	DECLARE @temp_member AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)
	DECLARE @temp_jd AS TABLE(split VARCHAR(100))
	INSERT INTO @temp_jd SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenisDebitur)
	DECLARE @temp_sc AS TABLE(split VARCHAR(5))
	INSERT INTO @temp_sc SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @statusCollectability) 
	DECLARE @temp_dc AS TABLE(split VARCHAR(5))
	INSERT INTO @temp_dc SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @dugaanCollectability) 

	SELECT rowid,
       kode_report,
       periode,
       member_type_code,
       member_code,
       status,
       collectibility_type_code,
	   dugaan_collectability,
       sum_outstanding,
       cnt_row,
       cnt_distinct_cif,
       cnt_acc,
       min_overdue_days,
       max_overdue_days,b.deskripsi_jenis_ljk 
	FROM dbo.BDA_F01_Predictive a
	LEFT JOIN dbo.master_ljk_type b ON a.member_type_code=b.kode_jenis_ljk
	WHERE periode BETWEEN @periode_awal AND @periode_akhir '
	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + 'AND a.member_type_code IN (SELECT split FROM @temp_memberType) '
	IF @ljk IS NOT NULL
		SET @sql = @sql + 'AND a.member_code IN (SELECT split FROM @temp_member) '
	IF @jenisDebitur IS NOT NULL
		SET @sql = @sql + 'AND a.status IN (SELECT split FROM @temp_jd) '
	IF @statusCollectability IS NOT NULL
		SET @sql = @sql + 'AND a.collectibility_type_code IN (SELECT split FROM @temp_sc) '
	IF @dugaanCollectability IS NOT NULL
		SET @sql = @sql + 'AND a.dugaan_collectability IN (SELECT split FROM @temp_dc) '

END 
EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20),@jenisDebitur VARCHAR(200), @statusCollectability VARCHAR(100), @dugaanCollectability VARCHAR(100) ', @jenis_ljk, @ljk, @periode_awal, @periode_akhir, @jenisDebitur, @statusCollectability, @dugaanCollectability
PRINT @sql
GO
