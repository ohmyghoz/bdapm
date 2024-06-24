SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION [dbo].[FT_Satker_Ancestor]
(	
	@sakterID BIGINT
)
RETURNS TABLE 
AS
RETURN 
(
	WITH w1( satker_id,satker_parent_id,satker_kode,satker_tipe,ref_kota_id,satker_nama,level ) AS 
	(		SELECT 
				satkerid,satkerparentid,satkerkode,satkertipe,refkotaid,satkernama,0
			FROM 
				dbo.MasterSatker t1
			WHERE 
				t1.satkerid = @sakterID 
		UNION ALL 
			SELECT t1.satkerid,t1.satkerparentid,t1.satkerkode,t1.satkertipe,t1.refkotaid,t1.satkernama,level +1
			FROM dbo.MasterSatker t1 JOIN w1 ON w1.satker_parent_id = t1.satkerid
	) 
	SELECT * FROM w1 
)
GO
