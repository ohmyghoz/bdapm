SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getLJKPengawas]
@userID VARCHAR(150)
AS 
SELECT member_code,pl.member_type_code,lt.deskripsi_jenis_ljk  FROM dbo.pengawas_ljk pl
LEFT JOIN dbo.master_ljk_type lt ON pl.member_type_code=lt.kode_jenis_ljk
WHERE pl.active_flag='Y' AND pl.user_login_id=@userID AND pl.member_code<>'' AND pl.member_type_code<>''
GO
