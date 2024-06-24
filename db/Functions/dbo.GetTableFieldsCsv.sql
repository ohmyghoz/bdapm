SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 26 April 2010
-- Description:	Mengembalikan field-field suatu tabel
-- =============================================
CREATE FUNCTION [dbo].[GetTableFieldsCsv]
(
	@table_name varchar(200)
)
RETURNS nvarchar(max)
AS
BEGIN
	select @table_name = lower(@table_name)
declare cur cursor for
SELECT  column_name=syscolumns.name, syscolumns.isnullable, systypes.name as typename, syscolumns.length
    FROM sysobjects 
    JOIN syscolumns ON sysobjects.id = syscolumns.id
    JOIN systypes ON syscolumns.xtype=systypes.xtype
   WHERE sysobjects.xtype='U' and sysobjects.name = @table_name
ORDER BY sysobjects.name,syscolumns.colid

declare @insertField varchar(max)
select @insertField = ''

declare @col_nama varchar(200)
declare @is_nullable int
declare @typename varchar(200)
declare @length bigint
OPEN cur

FETCH NEXT  from cur INTO @col_nama,@is_nullable,@typename,@length


WHILE @@FETCH_STATUS = 0
BEGIN
	SELECT @insertField = @insertField + ',' + @col_nama
FETCH NEXT  from cur INTO @col_nama,@is_nullable,@typename,@length

END

CLOSE cur
DEALLOCATE cur

SELECT @insertField = SUBSTRING(@insertField,2, LEN(@insertField)-1)
return @insertField
END






GO
