SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getSummaryRowTable]
@period DATE,
@tipe VARCHAR(50),
@jenis_ljk NVARCHAR(MAX) = NULL,
@ljk NVARCHAR(MAX) = NULL

AS 

DECLARE @sql NVARCHAR(MAX)
DECLARE @link  VARCHAR(250)=''
DECLARE @kode_jljk VARCHAR(50)=''
SELECT @link= SetValue FROM dbo.FWRefSetting WHERE SetName='Link_Prefix'
SET @sql = '
DECLARE @temp_memberType AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_memberType SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @jenis_ljk)
DECLARE @temp_member AS TABLE(split VARCHAR(200))
	INSERT INTO @temp_member SELECT LTRIM(RTRIM(split)) FROM dbo.Split('','', @ljk) option (maxrecursion 0)

SELECT a.TableName,c.menu_nama,c.judul,convert(varchar, FORMAT(d.Period, ''yyyy-MM'')) AS Period,REPLACE(f.ModMenuUrl,''~'',@link) +''?period='' + convert(varchar, d.Period, 23) '

IF @jenis_ljk IS NOT NULL
BEGIN
	SELECT @kode_jljk=kode_jenis_ljk FROM dbo.master_ljk_type WHERE deskripsi_jenis_ljk=@jenis_ljk
	SET @sql = @sql + '+ ''&jljk=' + @kode_jljk + ''''
END 
IF @ljk IS NOT NULL
BEGIN
	SELECT @kode_jljk=kode_jenis_ljk FROM dbo.master_ljk_type WHERE deskripsi_jenis_ljk=@jenis_ljk
	SET @sql = @sql + '+ ''&ljk=' + @kode_jljk+ ' - '' + @ljk '
END 
SET @sql = @sql + 'AS link,SUM(d.Rows) AS Rows FROM dbo.BDA2_Table a
LEFT JOIN dbo.osida_master c ON a.TableName=c.kode
LEFT JOIN dbo.BDA2_Table_Period_LJK d ON  d.TableName = a.TableName
LEFT JOIN dbo.FWRefModul e ON e.ModKode=c.menu_nama
LEFT JOIN dbo.FWRefModulMenu f ON f.ModId = e.ModId
WHERE d.Period=@period AND (a.TableName<>''osida_plafondering_umum_detail'' AND a.TableName NOT LIKE ''%_det'')  AND e.Stsrc=''A'' AND f.Stsrc=''A'' AND f.ModMenuIsHidden=0 AND a.TableName LIKE  @tipe+''%'' 
'
IF @jenis_ljk IS NOT NULL
	SET @sql = @sql + ' AND d.JenisLJK = @jenis_ljk '
IF @ljk IS NOT NULL
	SET @sql = @sql + ' AND d.KodeLJK = @ljk '
SET @sql = @sql + 'GROUP BY a.TableName,c.menu_nama,c.judul,d.Period,c.kode,f.ModMenuUrl 
				   ORDER BY c.menu_nama'
EXEC sys.sp_executesql @sql, N'@link VARCHAR(250), @jenis_ljk NVARCHAR(MAX), @ljk NVARCHAR(MAX), @period DATE, @tipe VARCHAR(50)', @link, @jenis_ljk, @ljk, @period, @tipe
PRINT @sql

GO
