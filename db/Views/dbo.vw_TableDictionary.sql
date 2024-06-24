SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO




CREATE VIEW [dbo].[vw_TableDictionary]
AS
SELECT x.DBName, x.TableName, x.SchemaName, x.ColumnName, x.DataType, x.AllowNull, x.DefaultText, 
Length = CASE WHEN x.DataType = 'nvarchar' THEN x.Length / 2 ELSE x.Length END, 
x.NumericPrecision, x.NumericScale, x.ColumnId 
FROM (
	SELECT      'BDA' AS DBName, tb.name AS TableName, s.name AS SchemaName,  CAST(c.name AS NVARCHAR(200)) AS ColumnName, 
				t.name AS DataType, c.is_nullable AS AllowNull, OBJECT_DEFINITION(c.default_object_id) AS DefaultText, 
				c.max_length AS Length, c.precision AS NumericPrecision, c.scale AS NumericScale, c.column_id ColumnId					  
	FROM        sys.columns AS c LEFT OUTER JOIN
				sys.tables AS tb ON tb.object_id = c.object_id LEFT OUTER JOIN
				sys.types AS t ON c.system_type_id = t.system_type_id AND c.user_type_id = t.user_type_id LEFT OUTER JOIN
				sys.schemas AS s ON tb.schema_id = s.schema_id                      
	WHERE     (tb.type = 'U') AND (tb.name <> 'sysdiagrams')
) x


GO
