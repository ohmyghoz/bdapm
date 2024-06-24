SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getDataCluster]
@periode_awal VARCHAR(20),
@periode_akhir VARCHAR(20),
@jenis_ljk NVARCHAR(MAX),
@ljk NVARCHAR(MAX),
@JenisAgunan NVARCHAR(MAX),
@Cluster VARCHAR(100),
@chart CHAR(1)
AS	
DECLARE @sql NVARCHAR(MAX)
SET @sql = ' '
IF @periode_awal IS NULL AND @periode_akhir IS NULL
BEGIN
SET @sql=@sql+' SELECT '''' AS jenis_agunan,'''' AS nama_agunan,'''' AS cluster,0.00 AS nilai_agunan,CONVERT(BIGINT,0) AS nilai_cnt'
END 
ELSE	
BEGIN
	SET @sql=@sql+'DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
	DECLARE @temp_member AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk)
	DECLARE @temp_ja AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_ja SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @JenisAgunan)
	DECLARE @temp_cluster AS TABLE(split VARCHAR(5))
	INSERT INTO @temp_cluster SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @Cluster) '
	
	IF @chart='Y'
	BEGIN	
	SET @sql=@sql+'SELECT a.jenis_agunan,b.nama_agunan,'''' AS cluster, '
	END
	ELSE
	BEGIN
	SET @sql=@sql+'SELECT a.jenis_agunan,b.nama_agunan,a.cluster, '
	END
			
	
	SET @sql=@sql+'SUM(a.sum_collateral_value_member) AS nilai_agunan,SUM(a.cnt) AS nilai_cnt FROM dbo.BDA_A01_Cluster a
	LEFT JOIN dbo.Master_Agunan b ON b.jenis_agunan = a.jenis_agunan
	WHERE periode BETWEEN @periode_awal AND @periode_akhir AND cluster IS NOT NULL '

	IF @jenis_ljk IS NOT NULL
		SET @sql = @sql + 'AND a.member_type_code IN (SELECT split FROM @temp_memberType) '
	IF @ljk IS NOT NULL
		SET @sql = @sql + 'AND a.member_code IN (SELECT split FROM @temp_member) '
	IF @JenisAgunan IS NOT NULL
		SET @sql = @sql + 'AND a.jenis_agunan IN (SELECT split FROM @temp_ja) '
	IF @Cluster IS NOT NULL
		SET @sql = @sql + 'AND a.cluster IN (SELECT split FROM @temp_cluster) '
	IF @chart='Y'
	BEGIN	
	SET @sql =@sql +'GROUP BY a.jenis_agunan,b.nama_agunan '
	END
	ELSE
	BEGIN
	SET @sql =@sql +'GROUP BY a.jenis_agunan,b.nama_agunan,a.cluster '
	END
	
END

EXEC sys.sp_executesql @sql, N'@jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @periode_awal VARCHAR(20), @periode_akhir VARCHAR(20),@JenisAgunan NVARCHAR(MAX), @Cluster VARCHAR(100) ', @jenis_ljk, @ljk, @periode_awal, @periode_akhir, @JenisAgunan, @Cluster
PRINT @sql

GO
