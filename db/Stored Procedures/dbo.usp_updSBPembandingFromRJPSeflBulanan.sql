SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_updSBPembandingFromRJPSeflBulanan]  
@periode_from datetime = '2019-01-01'  
AS  
  
  
declare @periode_until datetime  
select @periode_until = dateadd(day,-1,DATEADD(month,1,@periode_from))  
UPDATE Alert_Summary set NILAI_PEMBANDING1 = x.NILAI_PEMBANDING1  
from (  
 select a.rowid, a.PERIODE,  
 --a.KODE_ALERT, a.TIPE_PERIODE, a.MEMBER_TYPE_CODE, a.MEMBER_CODE, a.DIMENSI1, a.LEVEL_ALERT, a.NILAI1, a.NILAI2, b.NILAI1,   
 b.NILAI_PEMBANDING1   
 from Alert_Summary a   
 inner join Alert_Summary b on a.PERIODE = b.PERIODE and a.TIPE_PERIODE = b.TIPE_PERIODE  and a.DIMENSI1 = b.DIMENSI1  
 and a.MEMBER_TYPE_CODE = b.MEMBER_TYPE_CODE and a.MEMBER_CODE = b.MEMBER_CODE  
 and b.KODE_ALERT = 'RJP_Rata2_Diri_Sendiri' and b.DIMENSI2 = 'Bulanan'  
 where (a.Periode between @periode_from and @periode_until)   
 and (b.Periode between @periode_from and @periode_until)   
 and a.KODE_ALERT like 'SB%'  
)x  
inner join Alert_Summary upd on x.PERIODE = upd.PERIODE and upd.rowid = x.rowid
GO
