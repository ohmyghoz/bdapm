SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getModulChildren]
(
    @node_id bigint
)
AS
BEGIN
    WITH w1( ModId, ParentModId, ModKode, level ) AS 
    (
        SELECT 
            ModId,
            ParentModId,
            ModKode, 
            0 AS level 
        FROM 
            FWRefModul t1
        WHERE 
            ModId = @node_id AND t1.Stsrc='A'
        
        UNION ALL 
        
        SELECT 
            t1.ModId, 
            t1.ParentModId, 
            t1.ModKode,
            level + 1
        FROM 
            FWRefModul t1 
            JOIN w1 ON w1.ModId = t1.ParentModId
        WHERE t1.Stsrc='A'
    ) 
    
    SELECT * FROM w1;
END
GO
