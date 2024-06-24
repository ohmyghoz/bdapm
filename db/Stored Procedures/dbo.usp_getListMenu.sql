SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getListMenu]
@tipe VARCHAR(50)
AS 
DECLARE @sql NVARCHAR(MAX)
SET @sql='
DECLARE @link  VARCHAR(250)=''''

SELECT @link= SetValue FROM dbo.FWRefSetting WHERE SetName=''Link_Prefix''

SELECT ISNULL(CAST(( ROW_NUMBER() OVER( ORDER BY a.ModId )) AS INT),0) AS menu_id,a.ModNama + ''('' + a.ModTooltip + '')'' AS menu_nama,REPLACE(b.ModMenuUrl,''~'',@link) AS menu_link FROM dbo.FWRefModul a
LEFT JOIN dbo.FWRefModulMenu b ON b.ModId = a.ModId
WHERE a.Stsrc=''A'' AND b.Stsrc=''A''  AND b.ModMenuIsHidden=0
'
IF(@tipe='ms')
BEGIN
	SET @sql=@sql + 'AND ModKode LIKE ''ms0%'''
END 
ELSE IF(@tipe='ma')
BEGIN
	SET @sql=@sql + 'AND ModKode LIKE ''ma0%'' '
END 
ELSE IF(@tipe='micro')
BEGIN
	SET @sql=@sql + 'AND ModKode LIKE ''mip0%'''
END 
ELSE IF(@tipe='macro')
BEGIN
	SET @sql=@sql + 'AND ModKode LIKE ''map0%'''
END 
ELSE IF(@tipe='osida')
BEGIN
	SET @sql=@sql + 'AND ModKode LIKE ''osd%'''
END 
ELSE IF(@tipe='da')
BEGIN
	SET @sql=@sql + 'AND ModKode LIKE ''da%'''
END 
ELSE IF(@tipe='la')
BEGIN
	SET @sql=@sql + 'AND ModKode LIKE ''la%'''
END 
PRINT @sql
EXEC sys.sp_executesql @sql
GO
