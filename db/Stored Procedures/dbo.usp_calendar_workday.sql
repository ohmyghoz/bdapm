SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_calendar_workday] 
AS
BEGIN
	select workdayyear as Tahun, [Senin], [Selasa],[Rabu], [Kamis], [Jumat], [Sabtu], [Minggu]
	from (
		select workdayyear, workdaydayName, 'True' as exist
		from FWWorkday
		where stsrc = 'A'
	) as SourceTable
	pivot
	(
		max(exist) for workdaydayName in ([Senin], [Selasa],[Rabu], [Kamis], [Jumat], [Sabtu], [Minggu])
	) as pivotTable
	order by workdayyear
END
GO
