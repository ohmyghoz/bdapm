SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 2019-10-03
-- Description:	Melakukan email-email untuk si stakeholder ticket
-- =============================================
-- [usp_getNotification] 'admin'
CREATE PROCEDURE [dbo].[usp_getNotification]
@user_id VARCHAR(50)


AS
BEGIN

DECLARE @email VARCHAR(200)
SELECT @email = useremail FROM dbo.UserMaster WHERE userid = @user_id

SELECT TOP 20 noticeid, noticetitle, noticerefid, noticesmallcontent, nottid, CreatedDatetime
FROM dbo.FWNotice 
WHERE stsrc = 'A' AND (noticeto LIKE '%' + @email + '%' OR noticecc LIKE '%' + @email + '%')
ORDER BY CreatedDatetime DESC

END




GO
