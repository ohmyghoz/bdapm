SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getListQueue]
AS
DECLARE @rp VARCHAR(150)
SELECT @rp=SetValue FROM dbo.FWRefSetting WHERE SetName='RGQPath'
SELECT rgq_id,
       rg_id,
       rgq_query,
       rgq_params,
       rgq_nama,
       rgq_requestor,
       rgq_date,
       rgq_start,
       rgq_end,
       rgq_status,
       rgq_priority,
       rgq_urut,
       rgq_error_message,
       rgq_result_filesize,
       REPLACE(rgq_result_filename,@rp,'') AS rgq_result_filename,
       rgq_result_rowcount,
       stsrc,
       created_by,
       date_created,
       modified_by,
       date_modified,
       rgq_tablename FROM dbo.RptGrid_Queue
WHERE stsrc='A' 
GO
