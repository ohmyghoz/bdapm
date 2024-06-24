SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [dbo].[usp_getTotalPelapor]
@userID VARCHAR(150)
AS	
SELECT COUNT(DISTINCT member_type_code+member_code) AS totalPelapor FROM dbo.pengawas_ljk WHERE active_flag='Y' AND user_login_id =@userID

GO
