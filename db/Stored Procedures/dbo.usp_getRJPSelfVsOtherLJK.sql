SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_getRJPSelfVsOtherLJK]
@member_type_code varchar(4) = '0101',
@member_code varchar(6) = '016',
@member_type_code2 varchar(4) = '0101',
@member_code2 varchar(4) = '008',
@tipe_periode varchar(20) = 'Harian',
@dimensi1 varchar(50) = 'Interactive',
@dimensi2 varchar(50) = 'Bulanan',
@periode_from date = '2019-02-01',
@periode_until date = '2019-02-28'

AS


declare @red_iqr float = 1.5
declare @yellow_iqr float = 2.5

IF OBJECT_ID('tempdb..#tempIQR1') IS NOT NULL
    DROP TABLE #tempIQR1

IF OBJECT_ID('tempdb..#tempIQR2') IS NOT NULL
    DROP TABLE #tempIQR2

IF OBJECT_ID('tempdb..#tempIQRGab') IS NOT NULL
    DROP TABLE #tempIQRGab

IF OBJECT_ID('tempdb..#tempIQR') IS NOT NULL
    DROP TABLE #tempIQR

select rowid, a.tgl, NILAI1, NILAI_PEMBANDING1 as avg1
into #tempIQR1
from BDA.dbo.DimTanggal a
left join 
(
	SELECT rowid, PERIODE, NILAI1, NILAI_PEMBANDING1 FROM Alert_Summary 
	WHERE KODE_ALERT like 'RJP_Rata2_Diri_Sendiri' AND MEMBER_TYPE_CODE = @member_type_code and MEMBER_CODE = @member_code 
	and TIPE_PERIODE = @tipe_periode  and DIMENSI1 = @dimensi1  AND DIMENSI2 = @dimensi2
) x on a.tgl = x.PERIODE
where a.tgl between @periode_from and @periode_until AND (DATEPART(DAY,a.tgl) = 1 OR @tipe_periode = 'Harian')
order by tgl

select a.tgl, NILAI_PEMBANDING1 as avg2
into #tempIQR2
from DimTanggal a
left join 
(
	SELECT PERIODE, NILAI_PEMBANDING1 FROM Alert_Summary 
	WHERE KODE_ALERT like 'RJP_Rata2_Diri_Sendiri' AND MEMBER_TYPE_CODE = @member_type_code2 and MEMBER_CODE = @member_code2 
	and TIPE_PERIODE = @tipe_periode  and DIMENSI1 = @dimensi1  AND DIMENSI2 = @dimensi2
) x on a.tgl = x.PERIODE
where a.tgl between @periode_from and @periode_until AND (DATEPART(DAY,a.tgl) = 1 OR @tipe_periode = 'Harian')
order by tgl


-- isi yang null dengan value sebelumnya
Update #tempIQR1 set avg1 = x.bavg1
from (
	select a.tgl,  b.avg1 as bavg1, ROW_NUMBER() over (partition by (a.tgl) order by b.tgl desc ) urut --, a.avg1, b.tgl as btgl
	from #tempIQR1 a
	left join #tempIQR1 b on a.tgl > b.tgl and b.avg1 is not null
	where a.avg1 is null
)x 
inner join #tempIQR1 a on x.tgl = a.tgl and x.urut = 1
where a.avg1 is null


Update #tempIQR2 set avg2 = coalesce(x.bavg2,0)
from (
	select a.tgl,  b.avg2 as bavg2, ROW_NUMBER() over (partition by (a.tgl) order by b.tgl desc ) urut --, a.avg2, b.tgl as btgl
	from #tempIQR2 a
	left join #tempIQR2 b on a.tgl > b.tgl and b.avg2 is not null
	where a.avg2 is null
)x 
inner join #tempIQR2 a on x.tgl = a.tgl and x.urut = 1
where a.avg2 is null


select a.rowid, a.tgl, a.NILAI1, avg1, avg2, case when avg2 = 0 then 0 else avg1/avg2 end as ratio, cast(null as varchar(20)) as KODE_ALERT
into #tempIQRGab
from #tempIQR1 a
inner join #tempIQR2 b on a.tgl = b.tgl
order by a.tgl



SELECT TOP 1
Percentile_Cont(0.25) WITHIN GROUP(Order By ratio) OVER() As Q1,
Percentile_Cont(0.50) WITHIN GROUP(Order By ratio) OVER() As Median,
Percentile_Cont(0.75) WITHIN GROUP(Order By ratio) OVER() As Q3 ,
IQR = Percentile_Cont(0.75) WITHIN GROUP(Order By ratio) OVER() - Percentile_Cont(0.25) WITHIN GROUP(Order By ratio) OVER()
into #tempIQR
from #tempIQRGab


-- red dulu
update #tempIQRGab set KODE_ALERT = 3 where tgl in (
	SELECT tgl FROM #tempIQRGab, #tempIQR
	WHERE ratio > (Q3 + (IQR * @red_iqr))
) and KODE_ALERT IS NULL

-- yellow
update #tempIQRGab set KODE_ALERT = 2 where tgl in (
	SELECT tgl FROM #tempIQRGab, #tempIQR
	WHERE ratio > (Q3 + (IQR * @yellow_iqr))
) and KODE_ALERT IS NULL

update #tempIQRGab set KODE_ALERT = 1 where KODE_ALERT is null

select * from #tempIQRGab order by tgl
GO
