SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		Yulius
-- Create date: 2019-10-03
-- Description:	Melakukan email-email untuk si stakeholder ticket
-- =============================================
CREATE PROCEDURE [dbo].[usp_getRoleWithSatkerEmail]
@roleCsv VARCHAR(200),
@satker_id BIGINT

AS
BEGIN

DECLARE @emailCsv VARCHAR(4000) = ''
SELECT @emailCsv = @emailCsv + ',' + COALESCE(um.useremail,'') FROM dbo.FWRefRole r
INNER JOIN (SELECT DISTINCT split FROM dbo.Split(',',@roleCsv) WHERE LTRIM(split) <> '') x ON r.roleid = x.split
INNER JOIN dbo.FWUserRole ur ON ur.roleid = r.roleid
INNER JOIN dbo.UserMaster um ON um.userid = ur.userid
WHERE um.stsrc = 'A' AND ur.stsrc = 'A' AND r.stsrc = 'A' AND um.satkerid = @satker_id
AND COALESCE(um.useremail,'') <> ''


SELECT DISTINCT split AS email FROM dbo.Split(',',@emailCsv) WHERE LTRIM(split) <> ''


END




GO
